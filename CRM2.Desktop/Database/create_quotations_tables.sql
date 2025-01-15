USE crm;

-- Drop existing tables if they exist
DROP TABLE IF EXISTS quote_line_items;
DROP TABLE IF EXISTS quotations;

-- Create Quotations table
CREATE TABLE IF NOT EXISTS quotations (
    quote_id VARCHAR(20) COLLATE utf8mb4_unicode_ci PRIMARY KEY,
    date_created DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    customer_id VARCHAR(50) COLLATE utf8mb4_unicode_ci NOT NULL,
    customer_comments TEXT COLLATE utf8mb4_unicode_ci,
    internal_comments TEXT COLLATE utf8mb4_unicode_ci,
    currency_code CHAR(3) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'USD',
    status VARCHAR(20) COLLATE utf8mb4_unicode_ci NOT NULL DEFAULT 'DRAFT',
    created_by VARCHAR(50) COLLATE utf8mb4_unicode_ci,
    last_modified DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    last_modified_by VARCHAR(50) COLLATE utf8mb4_unicode_ci,
    CONSTRAINT valid_currency_code CHECK (currency_code REGEXP '^[A-Z]{3}$'),
    FOREIGN KEY (customer_id) REFERENCES contacts(contact_id) ON DELETE RESTRICT ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create Line Items table
CREATE TABLE IF NOT EXISTS quote_line_items (
    line_item_id INT AUTO_INCREMENT PRIMARY KEY,
    quote_id VARCHAR(20) COLLATE utf8mb4_unicode_ci NOT NULL,
    line_number INT NOT NULL,
    quantity DECIMAL(10,2) NOT NULL DEFAULT 1,
    part_number VARCHAR(50) COLLATE utf8mb4_unicode_ci NOT NULL,
    description TEXT COLLATE utf8mb4_unicode_ci NOT NULL,
    sell_price DECIMAL(15,2) NOT NULL,
    buy_price DECIMAL(15,2) NOT NULL,
    alternative_part_number VARCHAR(50) COLLATE utf8mb4_unicode_ci,
    supplier_code VARCHAR(20) COLLATE utf8mb4_unicode_ci,
    currency_code CHAR(3) COLLATE utf8mb4_unicode_ci NOT NULL,
    comments TEXT COLLATE utf8mb4_unicode_ci,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_modified DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT valid_quantity CHECK (quantity > 0),
    CONSTRAINT valid_prices CHECK (sell_price >= 0 AND buy_price >= 0),
    CONSTRAINT valid_line_currency CHECK (currency_code REGEXP '^[A-Z]{3}$'),
    UNIQUE(quote_id, line_number),
    FOREIGN KEY (quote_id) REFERENCES quotations(quote_id) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Create indexes for better performance
CREATE INDEX idx_quotations_customer_id ON quotations(customer_id);
CREATE INDEX idx_quote_line_items_quote_id ON quote_line_items(quote_id);
CREATE INDEX idx_quote_line_items_part_number ON quote_line_items(part_number);

-- Create a trigger for generating quote_id
DELIMITER //
CREATE TRIGGER before_insert_quotations
BEFORE INSERT ON quotations
FOR EACH ROW
BEGIN
    DECLARE next_id INT;
    SET next_id = (SELECT IFNULL(MAX(CAST(SUBSTRING(quote_id, 2) AS UNSIGNED)), 999) + 1 FROM quotations);
    SET NEW.quote_id = CONCAT('Q', LPAD(next_id, 6, '0'));
END//
DELIMITER ; 