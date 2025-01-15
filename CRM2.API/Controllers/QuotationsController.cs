using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using CRM2.API.Models;

namespace CRM2.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuotationsController : ControllerBase
{
    private readonly string _connectionString;

    public QuotationsController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string not found.");
        Console.WriteLine($"Using connection string: {_connectionString}");
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuote([FromBody] QuoteDto quote)
    {
        try
        {
            if (quote == null)
            {
                Console.WriteLine("Error: Quote data is null");
                return BadRequest("Quote data cannot be null");
            }

            if (string.IsNullOrEmpty(quote.CustomerId))
            {
                Console.WriteLine("Error: CustomerId is null or empty");
                return BadRequest("CustomerId is required");
            }

            if (string.IsNullOrEmpty(quote.CurrencyCode))
            {
                Console.WriteLine("Error: CurrencyCode is null or empty");
                return BadRequest("CurrencyCode is required");
            }

            // Log incoming data
            var debugInfo = new
            {
                CustomerId = quote.CustomerId,
                CurrencyCode = quote.CurrencyCode,
                LineItemCount = quote.LineItems?.Count ?? 0,
                LineItems = quote.LineItems?.Select(item => new
                {
                    PartNumber = item.PartNumber,
                    Description = item.Description,
                    Quantity = item.Quantity,
                    UnitOfMeasure = item.UnitOfMeasure,
                    SellPrice = item.SellPrice
                }).ToList()
            };
            Console.WriteLine($"Received quote data: {System.Text.Json.JsonSerializer.Serialize(debugInfo)}");

            // Test database connection first
            Console.WriteLine("Testing database connection...");
            await using (var testConnection = new MySqlConnection(_connectionString))
            {
                try
                {
                    await testConnection.OpenAsync();
                    Console.WriteLine("Database connection test successful");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database connection test failed: {ex.Message}");
                    throw new Exception("Failed to connect to database", ex);
                }
            }

            await using var connection = new MySqlConnection(_connectionString);
            Console.WriteLine("Opening database connection...");
            await connection.OpenAsync();
            Console.WriteLine("Database connection opened successfully");

            await using var transaction = await connection.BeginTransactionAsync();
            Console.WriteLine("Transaction started");

            try
            {
                // Verify customer exists
                Console.WriteLine($"Verifying customer {quote.CustomerId} exists...");
                await using var customerCheckCommand = new MySqlCommand(
                    "SELECT COUNT(*) FROM contacts WHERE contact_id = @customerId", connection, transaction);
                customerCheckCommand.Parameters.AddWithValue("@customerId", quote.CustomerId);
                var customerExists = Convert.ToInt32(await customerCheckCommand.ExecuteScalarAsync()) > 0;
                if (!customerExists)
                {
                    Console.WriteLine($"Error: Customer {quote.CustomerId} not found");
                    throw new Exception($"Customer with ID {quote.CustomerId} does not exist");
                }
                Console.WriteLine("Customer verification successful");

                // Get the next quote ID
                Console.WriteLine("Getting next quote ID...");
                await using var getNextIdCommand = new MySqlCommand(
                    @"SELECT COALESCE(MAX(CAST(SUBSTRING(quote_id, 2) AS UNSIGNED)), 999) + 1 FROM quotations", connection, transaction);
                var nextId = Convert.ToInt32(await getNextIdCommand.ExecuteScalarAsync());
                if (nextId < 1000) nextId = 1000;
                quote.QuoteId = $"Q{nextId:000000}";
                Console.WriteLine($"Generated quote ID: {quote.QuoteId}");

                // Insert the quote
                Console.WriteLine("Preparing to insert quote...");
                var insertQuoteSql = @"INSERT INTO quotations (
                    quote_id, 
                    customer_id, 
                    customer_comments, 
                    internal_comments, 
                    currency_code, 
                    status,
                    created_by,
                    last_modified_by,
                    date_created,
                    last_modified
                ) VALUES (
                    @quoteId,
                    @customerId,
                    @customerComments,
                    @internalComments,
                    @currencyCode,
                    @status,
                    'SYSTEM',
                    'SYSTEM',
                    UTC_TIMESTAMP(),
                    UTC_TIMESTAMP()
                )";
                Console.WriteLine($"Quote insert SQL: {insertQuoteSql}");

                await using var quoteCommand = new MySqlCommand(insertQuoteSql, connection, transaction);

                quoteCommand.Parameters.AddWithValue("@quoteId", quote.QuoteId);
                quoteCommand.Parameters.AddWithValue("@customerId", quote.CustomerId);
                quoteCommand.Parameters.AddWithValue("@customerComments", (object?)quote.CustomerComments ?? DBNull.Value);
                quoteCommand.Parameters.AddWithValue("@internalComments", (object?)quote.InternalComments ?? DBNull.Value);
                quoteCommand.Parameters.AddWithValue("@currencyCode", quote.CurrencyCode);
                quoteCommand.Parameters.AddWithValue("@status", "DRAFT");

                Console.WriteLine("Quote parameters set. Executing insert...");
                await quoteCommand.ExecuteNonQueryAsync();
                Console.WriteLine("Quote inserted successfully");

                // Insert line items
                Console.WriteLine($"Preparing to insert {quote.LineItems?.Count ?? 0} line items...");
                var lineItemNumber = 1;
                foreach (var item in quote.LineItems)
                {
                    Console.WriteLine($"Processing line item {lineItemNumber}...");
                    var insertLineItemSql = @"INSERT INTO quote_line_items (
                        quote_id, 
                        line_number, 
                        part_number, 
                        description, 
                        alternative_part_number, 
                        supplier_code, 
                        quantity,
                        unit_of_measure,
                        buy_price, 
                        sell_price, 
                        currency_code, 
                        comments
                    )
                    VALUES (
                        @quoteId, 
                        @lineNumber, 
                        @partNumber, 
                        COALESCE(@description, ''), 
                        @alternativePartNumber, 
                        @supplierCode, 
                        COALESCE(@quantity, 1),
                        @unitOfMeasure,
                        COALESCE(@buyPrice, 0), 
                        COALESCE(@sellPrice, 0), 
                        @currencyCode, 
                        @comments
                    )";
                    Console.WriteLine($"Line item insert SQL: {insertLineItemSql}");

                    await using var lineItemCommand = new MySqlCommand(insertLineItemSql, connection, transaction);

                    lineItemCommand.Parameters.AddWithValue("@quoteId", quote.QuoteId);
                    lineItemCommand.Parameters.AddWithValue("@lineNumber", lineItemNumber++);
                    lineItemCommand.Parameters.AddWithValue("@partNumber", item.PartNumber);
                    lineItemCommand.Parameters.AddWithValue("@description", item.Description ?? string.Empty);
                    lineItemCommand.Parameters.AddWithValue("@alternativePartNumber", item.AlternativePartNumber ?? (object)DBNull.Value);
                    lineItemCommand.Parameters.AddWithValue("@supplierCode", item.SupplierCode ?? (object)DBNull.Value);
                    lineItemCommand.Parameters.AddWithValue("@quantity", item.Quantity);
                    lineItemCommand.Parameters.AddWithValue("@unitOfMeasure", item.UnitOfMeasure ?? "PCS");
                    lineItemCommand.Parameters.AddWithValue("@buyPrice", item.BuyPrice);
                    lineItemCommand.Parameters.AddWithValue("@sellPrice", item.SellPrice);
                    lineItemCommand.Parameters.AddWithValue("@currencyCode", item.CurrencyCode);
                    lineItemCommand.Parameters.AddWithValue("@comments", item.Comments ?? (object)DBNull.Value);

                    var parameterValues = lineItemCommand.Parameters.Cast<MySqlParameter>()
                        .Select(p => $"{p.ParameterName}: {p.Value}");
                    Console.WriteLine($"Line item parameters: {string.Join(", ", parameterValues)}");

                    Console.WriteLine("Executing line item insert...");
                    await lineItemCommand.ExecuteNonQueryAsync();
                    Console.WriteLine($"Line item {lineItemNumber - 1} inserted successfully");
                }

                Console.WriteLine("All line items inserted. Committing transaction...");
                await transaction.CommitAsync();
                Console.WriteLine("Transaction committed successfully");
                return Ok(quote);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in transaction: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner exception stack trace: {ex.InnerException.StackTrace}");
                }

                Console.WriteLine("Rolling back transaction...");
                await transaction.RollbackAsync();
                Console.WriteLine("Transaction rolled back");
                throw new Exception($"Error in transaction: {ex.Message}", ex);
            }
        }
        catch (MySqlException ex)
        {
            Console.WriteLine($"MySQL Exception: {ex.Message}");
            Console.WriteLine($"Error Code: {ex.Number}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                Console.WriteLine($"Inner exception stack trace: {ex.InnerException.StackTrace}");
            }
            return StatusCode(500, $"Database error: {ex.Message} (Error Code: {ex.Number})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Outer exception: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                Console.WriteLine($"Inner exception stack trace: {ex.InnerException.StackTrace}");
            }

            var errorMessage = $"Error creating quote: {ex.Message}";
            if (ex.InnerException != null)
            {
                errorMessage += $"\nInner Exception: {ex.InnerException.Message}";
            }
            return StatusCode(500, errorMessage);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetQuotes()
    {
        try
        {
            var quotes = new List<QuoteDto>();

            await using var connection = new MySqlConnection(_connectionString);
            try 
            {
                await connection.OpenAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to open database connection: {ex.Message}");
                return StatusCode(500, "Database connection failed");
            }

            await using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var sql = @"
                    SELECT 
                        q.quote_id,
                        q.customer_id,
                        c.organization_name as customer_name,
                        q.customer_comments,
                        q.internal_comments,
                        q.currency_code,
                        q.status,
                        q.created_by,
                        q.last_modified_by,
                        DATE_FORMAT(q.date_created, '%Y-%m-%d %H:%i:%s') as created_at,
                        DATE_FORMAT(q.last_modified, '%Y-%m-%d %H:%i:%s') as updated_at
                    FROM quotations q
                    LEFT JOIN contacts c ON q.customer_id = c.contact_id
                    ORDER BY q.date_created DESC";

                Console.WriteLine($"Executing GetQuotes SQL: {sql}");
                await using var command = new MySqlCommand(sql, connection, transaction);
                
                List<Exception> errors = new();
                await using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    try
                    {
                        var customerNameOrdinal = reader.GetOrdinal("customer_name");
                        var customerCommentsOrdinal = reader.GetOrdinal("customer_comments");
                        var internalCommentsOrdinal = reader.GetOrdinal("internal_comments");
                        var createdAtOrdinal = reader.GetOrdinal("created_at");
                        var updatedAtOrdinal = reader.GetOrdinal("updated_at");

                        var quoteId = reader.GetString("quote_id");
                        Console.WriteLine($"DEBUG: Reading quote_id from database: {quoteId}");

                        var quote = new QuoteDto
                        {
                            QuoteId = quoteId,
                            CustomerId = reader.GetString("customer_id"),
                            CustomerName = !reader.IsDBNull(customerNameOrdinal) ? reader.GetString(customerNameOrdinal) : null,
                            CustomerComments = !reader.IsDBNull(customerCommentsOrdinal) ? reader.GetString(customerCommentsOrdinal) : null,
                            InternalComments = !reader.IsDBNull(internalCommentsOrdinal) ? reader.GetString(internalCommentsOrdinal) : null,
                            CurrencyCode = reader.GetString("currency_code"),
                            Status = reader.GetString("status"),
                            CreatedBy = reader.GetString("created_by"),
                            LastModifiedBy = reader.GetString("last_modified_by"),
                            CreatedAt = !reader.IsDBNull(createdAtOrdinal) ? reader.GetString(createdAtOrdinal) : null,
                            UpdatedAt = !reader.IsDBNull(updatedAtOrdinal) ? reader.GetString(updatedAtOrdinal) : null
                        };
                        quotes.Add(quote);
                        Console.WriteLine($"Retrieved quote: {quote.QuoteId}, Customer: {quote.CustomerName}, Created: {quote.CreatedAt}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing quote: {ex.Message}");
                        errors.Add(ex);
                    }
                }
                await reader.CloseAsync();

                // Get line items for each quote
                foreach (var quote in quotes)
                {
                    await using var lineItemCommand = new MySqlCommand(
                        @"SELECT *
                        FROM quote_line_items
                        WHERE quote_id = @quoteId
                        ORDER BY line_number", connection, transaction);
                    lineItemCommand.Parameters.AddWithValue("@quoteId", quote.QuoteId);

                    await using var lineItemReader = await lineItemCommand.ExecuteReaderAsync();
                    var lineItems = new List<QuoteLineItemDto>();
                    while (await lineItemReader.ReadAsync())
                    {
                        try
                        {
                            lineItems.Add(new QuoteLineItemDto
                            {
                                QuoteId = lineItemReader.GetString("quote_id"),
                                LineNumber = lineItemReader.GetInt32("line_number"),
                                PartNumber = lineItemReader.GetString("part_number"),
                                Description = lineItemReader.IsDBNull(lineItemReader.GetOrdinal("description")) ? null : lineItemReader.GetString("description"),
                                AlternativePartNumber = lineItemReader.IsDBNull(lineItemReader.GetOrdinal("alternative_part_number")) ? null : lineItemReader.GetString("alternative_part_number"),
                                SupplierCode = lineItemReader.IsDBNull(lineItemReader.GetOrdinal("supplier_code")) ? null : lineItemReader.GetString("supplier_code"),
                                Quantity = lineItemReader.GetDecimal("quantity"),
                                UnitOfMeasure = lineItemReader.IsDBNull(lineItemReader.GetOrdinal("unit_of_measure")) ? null : lineItemReader.GetString("unit_of_measure"),
                                BuyPrice = lineItemReader.GetDecimal("buy_price"),
                                SellPrice = lineItemReader.GetDecimal("sell_price"),
                                CurrencyCode = lineItemReader.GetString("currency_code"),
                                Comments = lineItemReader.IsDBNull(lineItemReader.GetOrdinal("comments")) ? null : lineItemReader.GetString("comments")
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error processing line item for quote {quote.QuoteId}: {ex.Message}");
                            errors.Add(ex);
                        }
                    }
                    await lineItemReader.CloseAsync();
                    quote.LineItems = lineItems;
                }

                await transaction.CommitAsync();

                if (errors.Any())
                {
                    Console.WriteLine($"Encountered {errors.Count} errors while processing quotes");
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Error: {error.Message}");
                    }
                }

                return Ok(quotes);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error reading quotes: {ex.Message}");
                return StatusCode(500, $"Error reading quotes: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unhandled error in GetQuotes: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return StatusCode(500, $"Unhandled error: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetQuote(string id)
    {
        try
        {
            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new MySqlCommand(
                @"SELECT 
                    q.*,
                    c.organization_name as customer_name,
                    DATE_FORMAT(q.date_created, '%Y-%m-%d %H:%i:%s') as created_at,
                    DATE_FORMAT(q.last_modified, '%Y-%m-%d %H:%i:%s') as updated_at
                  FROM quotations q
                  LEFT JOIN contacts c ON q.customer_id = c.contact_id
                  WHERE q.quote_id = @quoteId", connection);

            command.Parameters.AddWithValue("@quoteId", id);

            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return NotFound();

            var quote = new QuoteDto
            {
                QuoteId = reader.GetString("quote_id"),
                CustomerId = reader.GetString("customer_id"),
                CustomerName = reader.IsDBNull(reader.GetOrdinal("customer_name")) ? null : reader.GetString("customer_name"),
                CustomerComments = reader.IsDBNull(reader.GetOrdinal("customer_comments")) ? null : reader.GetString("customer_comments"),
                InternalComments = reader.IsDBNull(reader.GetOrdinal("internal_comments")) ? null : reader.GetString("internal_comments"),
                CurrencyCode = reader.GetString("currency_code"),
                Status = reader.GetString("status"),
                CreatedBy = reader.GetString("created_by"),
                LastModifiedBy = reader.GetString("last_modified_by"),
                CreatedAt = reader.IsDBNull(reader.GetOrdinal("created_at")) ? null : reader.GetString("created_at"),
                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at")) ? null : reader.GetString("updated_at")
            };

            // Get line items
            await using var lineItemCommand = new MySqlCommand(
                @"SELECT *
                FROM quote_line_items
                WHERE quote_id = @quoteId
                ORDER BY line_number", connection);
            lineItemCommand.Parameters.AddWithValue("@quoteId", id);

            await using var lineItemReader = await lineItemCommand.ExecuteReaderAsync();
            var lineItems = new List<QuoteLineItemDto>();
            while (await lineItemReader.ReadAsync())
            {
                lineItems.Add(new QuoteLineItemDto
                {
                    QuoteId = lineItemReader.GetString("quote_id"),
                    LineNumber = lineItemReader.GetInt32("line_number"),
                    PartNumber = lineItemReader.GetString("part_number"),
                    Description = lineItemReader.IsDBNull(lineItemReader.GetOrdinal("description")) ? null : lineItemReader.GetString("description"),
                    AlternativePartNumber = lineItemReader.IsDBNull(lineItemReader.GetOrdinal("alternative_part_number")) ? null : lineItemReader.GetString("alternative_part_number"),
                    SupplierCode = lineItemReader.IsDBNull(lineItemReader.GetOrdinal("supplier_code")) ? null : lineItemReader.GetString("supplier_code"),
                    Quantity = lineItemReader.GetDecimal("quantity"),
                    UnitOfMeasure = lineItemReader.IsDBNull(lineItemReader.GetOrdinal("unit_of_measure")) ? null : lineItemReader.GetString("unit_of_measure"),
                    BuyPrice = lineItemReader.GetDecimal("buy_price"),
                    SellPrice = lineItemReader.GetDecimal("sell_price"),
                    CurrencyCode = lineItemReader.GetString("currency_code"),
                    Comments = lineItemReader.IsDBNull(lineItemReader.GetOrdinal("comments")) ? null : lineItemReader.GetString("comments")
                });
            }
            quote.LineItems = lineItems;

            return Ok(quote);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error retrieving quote: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuote(string id, [FromBody] QuoteDto quote)
    {
        if (id != quote.QuoteId)
            return BadRequest("Quote ID mismatch");

        try
        {
            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Update the quote
                await using var quoteCommand = new MySqlCommand(
                    @"UPDATE quotations 
                      SET customer_id = @customerId,
                          customer_comments = @customerComments,
                          internal_comments = @internalComments,
                          currency_code = @currencyCode,
                          status = @status
                      WHERE quote_id = @quoteId", connection, transaction);

                quoteCommand.Parameters.AddWithValue("@quoteId", id);
                quoteCommand.Parameters.AddWithValue("@customerId", quote.CustomerId);
                quoteCommand.Parameters.AddWithValue("@customerComments", quote.CustomerComments ?? (object)DBNull.Value);
                quoteCommand.Parameters.AddWithValue("@internalComments", quote.InternalComments ?? (object)DBNull.Value);
                quoteCommand.Parameters.AddWithValue("@currencyCode", quote.CurrencyCode);
                quoteCommand.Parameters.AddWithValue("@status", quote.Status);

                var rowsAffected = await quoteCommand.ExecuteNonQueryAsync();
                if (rowsAffected == 0)
                    return NotFound();

                // Delete existing line items
                await using var deleteCommand = new MySqlCommand(
                    "DELETE FROM quote_line_items WHERE quote_id = @quoteId", connection, transaction);
                deleteCommand.Parameters.AddWithValue("@quoteId", id);
                await deleteCommand.ExecuteNonQueryAsync();

                // Insert updated line items
                foreach (var item in quote.LineItems)
                {
                    await using var lineItemCommand = new MySqlCommand(
                        @"INSERT INTO quote_line_items (
                            quote_id, 
                            line_number, 
                            part_number, 
                            description, 
                            alternative_part_number, 
                            supplier_code, 
                            quantity,
                            unit_of_measure,
                            buy_price, 
                            sell_price, 
                            currency_code, 
                            comments
                        )
                        VALUES (
                            @quoteId, 
                            @lineNumber, 
                            @partNumber, 
                            COALESCE(@description, ''), 
                            @alternativePartNumber, 
                            @supplierCode, 
                            COALESCE(@quantity, 1),
                            @unitOfMeasure,
                            COALESCE(@buyPrice, 0), 
                            COALESCE(@sellPrice, 0), 
                            @currencyCode, 
                            @comments
                        )", connection, transaction);

                    lineItemCommand.Parameters.AddWithValue("@quoteId", id);
                    lineItemCommand.Parameters.AddWithValue("@lineNumber", item.LineNumber);
                    lineItemCommand.Parameters.AddWithValue("@partNumber", item.PartNumber);
                    lineItemCommand.Parameters.AddWithValue("@description", item.Description ?? string.Empty);
                    lineItemCommand.Parameters.AddWithValue("@alternativePartNumber", item.AlternativePartNumber ?? (object)DBNull.Value);
                    lineItemCommand.Parameters.AddWithValue("@supplierCode", item.SupplierCode ?? (object)DBNull.Value);
                    lineItemCommand.Parameters.AddWithValue("@quantity", item.Quantity);
                    lineItemCommand.Parameters.AddWithValue("@unitOfMeasure", item.UnitOfMeasure);
                    lineItemCommand.Parameters.AddWithValue("@buyPrice", item.BuyPrice);
                    lineItemCommand.Parameters.AddWithValue("@sellPrice", item.SellPrice);
                    lineItemCommand.Parameters.AddWithValue("@currencyCode", item.CurrencyCode);
                    lineItemCommand.Parameters.AddWithValue("@comments", item.Comments ?? (object)DBNull.Value);

                    await lineItemCommand.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return Ok(quote);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating quote: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuote(string id)
    {
        try
        {
            await using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new MySqlCommand(
                "DELETE FROM quotations WHERE quote_id = @quoteId", connection);
            command.Parameters.AddWithValue("@quoteId", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            if (rowsAffected == 0)
                return NotFound();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error deleting quote: {ex.Message}");
        }
    }
} 