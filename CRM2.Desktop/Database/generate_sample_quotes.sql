USE crm;

-- Allow creation of functions
SET GLOBAL log_bin_trust_function_creators = 1;

-- Drop existing functions and procedures
DROP FUNCTION IF EXISTS rand_text;
DROP FUNCTION IF EXISTS rand_part_number;
DROP FUNCTION IF EXISTS rand_supplier_code;
DROP FUNCTION IF EXISTS get_next_quote_id;
DROP PROCEDURE IF EXISTS generate_sample_quotes;

-- Disable foreign key checks temporarily
SET FOREIGN_KEY_CHECKS = 0;

-- Clear existing data
TRUNCATE TABLE quote_line_items;
TRUNCATE TABLE quotations;

-- Re-enable foreign key checks
SET FOREIGN_KEY_CHECKS = 1;

-- Create temporary table for storing random customer IDs
CREATE TEMPORARY TABLE temp_customer_ids AS
SELECT contact_id FROM contacts;

-- Helper function to get random text
DELIMITER //
CREATE FUNCTION IF NOT EXISTS rand_text(length INT) RETURNS TEXT
DETERMINISTIC
BEGIN
    DECLARE chars TEXT DEFAULT 'abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 ';
    DECLARE result TEXT DEFAULT '';
    DECLARE i INT DEFAULT 0;
    DECLARE char_pos INT;
    WHILE i < length DO
        SET char_pos = FLOOR(1 + RAND() * LENGTH(chars));
        SET result = CONCAT(result, SUBSTRING(chars, char_pos, 1));
        SET i = i + 1;
    END WHILE;
    RETURN result;
END //
DELIMITER ;

-- Helper function to get random part number
DELIMITER //
CREATE FUNCTION IF NOT EXISTS rand_part_number() RETURNS VARCHAR(50)
DETERMINISTIC
BEGIN
    DECLARE prefix CHAR(2);
    DECLARE number INT;
    DECLARE rand_val INT;
    SET rand_val = FLOOR(1 + RAND() * 5);
    SET prefix = CASE rand_val
        WHEN 1 THEN 'PT'
        WHEN 2 THEN 'SP'
        WHEN 3 THEN 'AC'
        WHEN 4 THEN 'EL'
        ELSE 'MT'
    END;
    SET number = FLOOR(10000 + RAND() * 90000);
    RETURN CONCAT(prefix, number);
END //
DELIMITER ;

-- Helper function to get random supplier code
DELIMITER //
CREATE FUNCTION IF NOT EXISTS rand_supplier_code() RETURNS VARCHAR(20)
DETERMINISTIC
BEGIN
    DECLARE prefix CHAR(3);
    DECLARE number INT;
    DECLARE rand_val INT;
    SET rand_val = FLOOR(1 + RAND() * 5);
    SET prefix = CASE rand_val
        WHEN 1 THEN 'SUP'
        WHEN 2 THEN 'VEN'
        WHEN 3 THEN 'MFG'
        WHEN 4 THEN 'DIS'
        ELSE 'RET'
    END;
    SET number = FLOOR(100 + RAND() * 900);
    RETURN CONCAT(prefix, number);
END //
DELIMITER ;

-- Helper function to get next quote ID
DELIMITER //
CREATE FUNCTION IF NOT EXISTS get_next_quote_id() RETURNS VARCHAR(20)
DETERMINISTIC
BEGIN
    DECLARE next_id INT;
    SET next_id = (SELECT IFNULL(MAX(CAST(SUBSTRING(quote_id, 2) AS UNSIGNED)), 999) + 1 FROM quotations);
    RETURN CONCAT('Q', LPAD(next_id, 6, '0'));
END //
DELIMITER ;

-- Generate 5000 quotes
DELIMITER //
CREATE PROCEDURE generate_sample_quotes()
BEGIN
    DECLARE i INT DEFAULT 0;
    DECLARE customer_count INT;
    DECLARE random_customer_id VARCHAR(50);
    DECLARE quote_id VARCHAR(20);
    DECLARE currency_code CHAR(3);
    DECLARE status VARCHAR(20);
    DECLARE line_items INT;
    DECLARE j INT;
    DECLARE rand_val INT;
    
    SELECT COUNT(*) INTO customer_count FROM temp_customer_ids;
    
    WHILE i < 5000 DO
        -- Get random customer ID
        SET rand_val = FLOOR(RAND() * customer_count);
        SELECT contact_id INTO random_customer_id 
        FROM temp_customer_ids 
        LIMIT 1 
        OFFSET rand_val;
        
        -- Get random currency code
        SET rand_val = FLOOR(1 + RAND() * 4);
        SET currency_code = CASE rand_val
            WHEN 1 THEN 'USD'
            WHEN 2 THEN 'EUR'
            WHEN 3 THEN 'GBP'
            ELSE 'JPY'
        END;
        
        -- Get random status
        SET rand_val = FLOOR(1 + RAND() * 4);
        SET status = CASE rand_val
            WHEN 1 THEN 'DRAFT'
            WHEN 2 THEN 'PENDING'
            WHEN 3 THEN 'APPROVED'
            ELSE 'REJECTED'
        END;
        
        -- Get next quote ID
        SET quote_id = get_next_quote_id();
        
        -- Insert quote
        INSERT INTO quotations (
            quote_id,
            customer_id,
            customer_comments,
            internal_comments,
            currency_code,
            status,
            created_by,
            last_modified_by
        ) VALUES (
            quote_id,
            random_customer_id,
            CASE WHEN RAND() < 0.7 THEN rand_text(50) ELSE NULL END,
            CASE WHEN RAND() < 0.5 THEN rand_text(50) ELSE NULL END,
            currency_code,
            status,
            'SYSTEM',
            'SYSTEM'
        );
        
        -- Generate random number of line items (1-20)
        SET line_items = 1 + FLOOR(RAND() * 20);
        SET j = 0;
        
        WHILE j < line_items DO
            INSERT INTO quote_line_items (
                quote_id,
                line_number,
                quantity,
                part_number,
                description,
                sell_price,
                buy_price,
                alternative_part_number,
                supplier_code,
                currency_code,
                comments
            ) VALUES (
                quote_id,
                j + 1,
                ROUND(1 + RAND() * 100, 2),
                rand_part_number(),
                rand_text(30),
                ROUND(10 + RAND() * 990, 2),
                ROUND(5 + RAND() * 800, 2),
                CASE WHEN RAND() < 0.3 THEN rand_part_number() ELSE NULL END,
                rand_supplier_code(),
                currency_code,
                CASE WHEN RAND() < 0.4 THEN rand_text(40) ELSE NULL END
            );
            
            SET j = j + 1;
        END WHILE;
        
        SET i = i + 1;
        
        IF i % 100 = 0 THEN
            SELECT CONCAT('Generated ', i, ' quotes') AS progress;
        END IF;
    END WHILE;
END //
DELIMITER ;

-- Execute the procedure
CALL generate_sample_quotes();

-- Clean up
DROP FUNCTION IF EXISTS rand_text;
DROP FUNCTION IF EXISTS rand_part_number;
DROP FUNCTION IF EXISTS rand_supplier_code;
DROP FUNCTION IF EXISTS get_next_quote_id;
DROP PROCEDURE IF EXISTS generate_sample_quotes;
DROP TEMPORARY TABLE IF EXISTS temp_customer_ids;

-- Reset function creation setting
SET GLOBAL log_bin_trust_function_creators = 0; 