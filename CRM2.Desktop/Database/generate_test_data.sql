-- Create temporary tables for sample data
CREATE TEMPORARY TABLE company_names (name VARCHAR(255));
INSERT INTO company_names VALUES
('Acme Corporation'), ('TechCorp Solutions'), ('Global Industries'),
('Innovative Systems'), ('Digital Dynamics'), ('Future Technologies'),
('Smart Solutions'), ('Elite Enterprises'), ('Prime Industries'),
('Advanced Analytics'), ('Bright Horizons'), ('Cloud Nine Computing'),
('Data Dynamics'), ('Echo Systems'), ('First Choice Tech');

CREATE TEMPORARY TABLE cities (name VARCHAR(100), country VARCHAR(100));
INSERT INTO cities VALUES
('New York', 'USA'), ('London', 'UK'), ('Tokyo', 'Japan'),
('Paris', 'France'), ('Berlin', 'Germany'), ('Sydney', 'Australia'),
('Toronto', 'Canada'), ('Singapore', 'Singapore'), ('Dubai', 'UAE'),
('Mumbai', 'India'), ('São Paulo', 'Brazil'), ('Amsterdam', 'Netherlands'),
('Stockholm', 'Sweden'), ('Hong Kong', 'China'), ('Seoul', 'South Korea');

CREATE TEMPORARY TABLE domains (domain VARCHAR(100));
INSERT INTO domains VALUES
('@techcorp.com'), ('@globalsys.net'), ('@innovate.io'),
('@smarttech.com'), ('@futuretech.co'), ('@brightdata.com'),
('@cloudcomp.net'), ('@elitetech.io'), ('@primeind.com'),
('@advtech.com'), ('@horizons.net'), ('@dynamics.co'),
('@echosys.com'), ('@firsttech.io'), ('@nexgen.com');

-- Generate 5000 contacts
DELIMITER //
CREATE PROCEDURE generate_contacts()
BEGIN
    DECLARE i INT DEFAULT 1;
    WHILE i <= 5000 DO
        -- Get random values from temporary tables
        SET @company_name = (SELECT name FROM company_names ORDER BY RAND() LIMIT 1);
        SET @city_data = (SELECT CONCAT_WS('|', name, country) FROM cities ORDER BY RAND() LIMIT 1);
        SET @domain = (SELECT domain FROM domains ORDER BY RAND() LIMIT 1);
        SET @city = SUBSTRING_INDEX(@city_data, '|', 1);
        SET @country = SUBSTRING_INDEX(@city_data, '|', -1);
        
        -- Generate contact data
        SET @contact_id = CONCAT('C', LPAD(i, 5, '0'));
        SET @contact_type = IF(RAND() < 0.7, 'CUSTOMER', 'SUPPLIER');
        SET @address = CONCAT(FLOOR(RAND() * 999 + 1), ' ', 
                            ELT(FLOOR(RAND() * 4) + 1, 'Main', 'Business', 'Corporate', 'Industry'), 
                            ' ', 
                            ELT(FLOOR(RAND() * 4) + 1, 'Street', 'Avenue', 'Boulevard', 'Road'));
        SET @postal_code = LPAD(FLOOR(RAND() * 99999), 5, '0');
        SET @phone = CONCAT('+', FLOOR(RAND() * 90 + 10), 
                          FLOOR(RAND() * 900 + 100), 
                          FLOOR(RAND() * 9000000 + 1000000));
        SET @email = CONCAT('contact', i, @domain);
        SET @website = CONCAT('www.', LOWER(REPLACE(@company_name, ' ', '')), '.com');
        SET @tax_id = CONCAT('TAX', LPAD(FLOOR(RAND() * 999999), 6, '0'));
        SET @tax_rate = ROUND(RAND() * 25, 2);
        
        -- Insert contact
        INSERT INTO contacts (
            contact_id, contact_type, organization_name,
            address_line1, postal_code, city, country,
            email, phone_number, website_url, tax_id, tax_rate
        ) VALUES (
            @contact_id, @contact_type, @company_name,
            @address, @postal_code, @city, @country,
            @email, @phone, @website, @tax_id, @tax_rate
        );
        
        SET i = i + 1;
    END WHILE;
END //
DELIMITER ;

-- Execute the procedure
CALL generate_contacts();

-- Clean up
DROP PROCEDURE IF EXISTS generate_contacts;
DROP TEMPORARY TABLE IF EXISTS company_names;
DROP TEMPORARY TABLE IF EXISTS cities;
DROP TEMPORARY TABLE IF EXISTS domains; 