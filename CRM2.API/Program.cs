using Microsoft.Data.SqlClient;

namespace CRM2.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Test database connection and structure
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Successfully connected to database!");

                // Query to get ContactInfo table structure
                string columnQuery = @"
                    SELECT 
                        COLUMN_NAME,
                        DATA_TYPE,
                        CHARACTER_MAXIMUM_LENGTH,
                        IS_NULLABLE,
                        COLUMN_DEFAULT
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'ContactInfo'
                    ORDER BY ORDINAL_POSITION";

                Console.WriteLine("\nContactInfo Table Structure:");
                using (var command = new SqlCommand(columnQuery, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string columnName = reader["COLUMN_NAME"].ToString();
                        string dataType = reader["DATA_TYPE"].ToString();
                        var maxLength = reader["CHARACTER_MAXIMUM_LENGTH"];
                        string nullable = reader["IS_NULLABLE"].ToString();
                        var defaultValue = reader["COLUMN_DEFAULT"];

                        string lengthStr = maxLength == DBNull.Value ? "" : $"({maxLength})";
                        string nullableStr = nullable == "YES" ? "nullable" : "not null";
                        string defaultStr = defaultValue == DBNull.Value ? "" : $" default: {defaultValue}";

                        Console.WriteLine($"- {columnName}: {dataType}{lengthStr}, {nullableStr}{defaultStr}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
