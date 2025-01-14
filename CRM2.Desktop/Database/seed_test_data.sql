-- First names array
SET @first_names = 'James,John,Robert,Michael,William,David,Richard,Joseph,Thomas,Charles,Christopher,Daniel,Matthew,Anthony,Donald,Mark,Paul,Steven,Andrew,Kenneth,Joshua,Kevin,Brian,George,Edward,Ronald,Timothy,Jason,Jeffrey,Ryan,Jacob,Gary,Nicholas,Eric,Jonathan,Stephen,Larry,Justin,Scott,Brandon,Benjamin,Samuel,Gregory,Frank,Alexander,Raymond,Patrick,Jack,Dennis,Jerry,Tyler,Aaron,Jose,Adam,Nathan,Henry,Douglas,Zachary,Peter,Kyle,Mary,Patricia,Jennifer,Linda,Elizabeth,Barbara,Susan,Jessica,Sarah,Karen,Nancy,Lisa,Betty,Margaret,Sandra,Ashley,Kimberly,Emily,Donna,Michelle,Carol,Amanda,Dorothy,Melissa,Deborah,Stephanie,Rebecca,Sharon,Laura,Cynthia,Kathleen,Amy,Angela,Shirley,Anna,Brenda,Pamela,Emma,Nicole,Helen,Samantha,Katherine,Christine,Debra,Rachel,Carolyn,Janet,Catherine,Maria,Heather,Diane,Ruth,Julie,Olivia,Joyce,Virginia,Victoria,Kelly,Lauren,Christina,Joan,Evelyn,Judith,Megan,Andrea,Cheryl,Hannah';

-- Last names array
SET @last_names = 'Smith,Johnson,Williams,Brown,Jones,Garcia,Miller,Davis,Rodriguez,Martinez,Hernandez,Lopez,Gonzalez,Wilson,Anderson,Thomas,Taylor,Moore,Jackson,Martin,Lee,Perez,Thompson,White,Harris,Sanchez,Clark,Ramirez,Lewis,Robinson,Walker,Young,Allen,King,Wright,Scott,Torres,Nguyen,Hill,Flores,Green,Adams,Nelson,Baker,Hall,Rivera,Campbell,Mitchell,Carter,Roberts,Turner,Phillips,Parker,Evans,Edwards,Collins,Stewart,Morris,Murphy,Cook,Rogers,Morgan,Peterson,Cooper,Reed,Bailey,Bell,Gomez,Kelly,Howard,Ward,Cox,Diaz,Richardson,Wood,Watson,Brooks,Bennett,Gray,James,Reyes,Cruz,Hughes,Price,Myers,Long,Foster,Sanders,Ross,Morales,Powell,Sullivan,Russell,Ortiz,Jenkins,Gutierrez,Perry,Butler,Barnes,Fisher,Henderson,Coleman,Simmons,Patterson,Jordan,Reynolds,Hamilton,Graham,Kim,Gonzales,Alexander,Ramos,Wallace,Griffin,West,Cole,Hayes,Chavez,Gibson,Bryant,Ellis,Stevens,Murray,Ford,Marshall,Owens,McDonald,Harrison,Ruiz,Kennedy,Wells,Alvarez,Woods,Mendoza,Castillo,Olson,Webb,Washington,Tucker,Freeman,Burns,Henry,Vasquez,Snyder,Simpson,Crawford,Jimenez,Porter,Mason,Shaw,Gordon,Wagner,Hunter,Romero,Hicks,Dixon,Hunt,Palmer,Robertson,Black,Holmes,Stone,Meyer,Boyd,Mills,Warren,Fox,Rose,Rice,Moreno,Schmidt,Patel,Ferguson,Nichols,Herrera,Medina,Ryan,Fernandez,Weaver,Daniels,Stephens,Gardner,Payne,Kelley,Dunn,Pierce,Arnold,Tran,Spencer,Peters,Hawkins,Grant,Hansen,Castro,Hoffman,Hart,Elliott,Cunningham,Knight,Bradley,Carroll,Hudson,Duncan,Armstrong,Berry,Andrews,Johnston,Ray,Lane,Riley,Carpenter,Perkins,Aguilar,Silva,Richards,Willis,Matthews,Chapman,Lawrence,Garza,Vargas,Watkins,Wheeler,Larson,Carlson,Harper,George,Greene,Burke,Guzman,Morrison,Munoz,Jacobs,Obrien,Lawson,Franklin,Lynch,Bishop,Carr,Salazar,Austin,Mendez,Gilbert,Jensen,Williamson,Montgomery,Harvey,Oliver,Howell,Dean,Hanson,Weber,Garrett,Sims,Burton,Fuller,Soto,McCoy,Welch,Chen,Schultz,Walters,Reid,Fields,Walsh,Little,Fowler,Bowman,Davidson,May,Day,Schneider,Newman,Brewer,Lucas,Holland,Wong,Banks,Santos,Curtis,Pearson,Delgado,Valdez,Pena,Rios,Douglas,Sandoval,Barrett,Hopkins,Keller,Guerrero,Stanley,Bates,Alvarado,Beck,Ortega,Wade,Estrada,Contreras,Barnett,Caldwell,Santiago,Lambert,Powers,Chambers,Nunez,Craig,Leonard,Lowe,Rhodes,Byrd,Gregory,Shelton,Frazier,Becker,Maldonado,Fleming,Vega,Sutton,Cohen,Jennings,Parks,McDaniel,Watts,Barker,Norris,Vaughn,Vazquez,Holt,Schwartz,Steele,Benson,Neal,Dominguez,Horton,Terry,Wolfe,Hale,Lyons,Graves,Haynes,Miles,Park,Warner,Padilla,Bush,Thornton,McCarthy,Mann,Zimmerman,Erickson,Fletcher,McKinney,Page,Dawson,Joseph,Marquez,Reeves,Klein,Espinoza,Baldwin,Moran,Love,Robbins,Higgins,Ball,Cortez,Le,Griffith,Bowen,Sharp,Cummings,Ramsey,Hardy,Swanson,Barber,Acosta,Luna,Chandler,Blair,Daniel,Cross,Simon,Dennis,Oconnor,Quinn,Gross,Navarro,Moss,Fitzgerald,Doyle,McLaughlin,Rojas,Rodgers,Stevenson,Singh,Yang,Figueroa,Harmon,Newton,Paul,Manning,Garner,McGee,Reese,Francis,Burgess,Adkins,Goodman,Curry,Brady,Christensen,Potter,Walton,Goodwin,Mullins,Molina,Webster,Fischer,Campos,Avila,Sherman,Todd,Chang,Blake,Malone,Wolf,Hodges,Juarez,Gill,Farmer,Hines,Gallagher,Duran,Hubbard,Cannon,Miranda,Wang,Saunders,Tate,Mack,Hammond,Carrillo,Townsend,Wise,Ingram,Barton,Mejia,Ayala,Schroeder,Hampton,Rowe,Parsons,Frank,Waters,Strickland,Osborne,Maxwell,Chan,Deleon,Norman,Harrington,Casey,Patton,Logan,Bowers,Mueller,Glover,Floyd,Hartman,Buchanan,Cobb,French,Kramer,McCormick,Clarke,Tyler,Gibbs,Moody,Conner,Sparks,McGuire,Leon,Bauer,Norton,Pope,Flynn,Hogan,Robles,Salinas,Yates,Lindsey,Lloyd,Marsh,McBride,Owen,Solis,Pham,Lang,Pratt,Lara,Brock,Ballard,Trujillo,Shaffer,Drake,Roman,Aguirre,Morton,Stokes,Lamb,Pacheco,Patrick,Cochran,Shepherd,Cain,Burnett,Hess,Li,Cervantes,Olsen,Briggs,Ochoa,Cabrera,Velasquez,Montoya,Roth,Meyers,Cardenas,Fuentes,Weiss,Wilkins,Hoover,Nicholson,Underwood,Short,Carson,Morrow,Colon,Holloway,Summers,Bryan,Petersen,McKenzie,Serrano,Wilcox,Carey,Clayton,Poole,Calderon,Gallegos,Greer,Rivas,Guerra,Decker,Collier,Wall,Whitaker,Bass,Flowers,Davenport,Conley,Houston,Huff,Copeland,Hood,Monroe,Massey,Roberson,Combs,Franco,Larsen,Pittman,Randall,Skinner,Wilkinson,Kirby,Cameron,Bridges,Anthony,Richard,Kirk,Bruce,Singleton,Mathis,Bradford,Boone,Abbott,Charles,Allison,Sweeney,Atkinson,Horn,Jefferson,Rosales,York,Christian,Phelps,Farrell,Castaneda,Nash,Dickerson,Bond,Wyatt,Foley,Chase,Gates,Vincent,Mathews,Hodge,Garrison,Trevino,Villarreal,Heath,Dalton,Valencia,Callahan,Hensley,Atkins,Huffman,Roy,Boyer,Shields,Lin,Hancock,Grimes,Glenn,Cline,Delacruz,Camacho,Dillon,Parrish,Oneill,Melton,Booth,Kane,Berg,Harrell,Pitts,Savage,Wiggins,Brennan,Salas,Marks,Russo,Sawyer,Baxter,Golden,Hutchinson,Liu,Walter,McDowell,Wiley,Rich,Humphrey,Johns,Koch,Suarez,Hobbs,Beard,Gilmore,Ibarra,Keith,Macias,Khan,Andrade,Ware,Stephenson,Henson,Wilkerson,Dyer,McClure,Blackwell,Mercado,Tanner,Eaton,Clay,Barron,Beasley,Oneal,Small,Preston,Wu,Zamora,Macdonald,Vance,Snow,McClain,Stafford,Orozco,Barry,English,Shannon,Kline,Jacobson,Woodard,Huang,Kemp,Mosley,Prince,Merritt,Hurst,Villanueva,Roach,Nolan,Lam,Yoder,McCullough,Lester,Santana,Valenzuela,Winters,Barrera,Orr,Leach,Berger,McKee,Strong,Conway,Stein,Whitehead,Bullock,Escobar,Knox,Meadows,Solomon,Velez,Odonnell,Kerr,Stout,Blankenship,Browning,Kent,Lozano,Bartlett,Pruitt,Buck,Barr,Gaines,Durham,Gentry,McIntyre,Sloan,Rocha,Melendez,Herman,Sexton,Moon,Hendricks,Rangel,Stark,Lowery,Hardin,Hull,Sellers,Ellison,Calhoun,Gillespie,Mora,Knapp,McCall,Morse,Dorsey,Weeks,Nielsen,Livingston,Leblanc,McLean,Bradshaw,Glass,Middleton,Buckley,Schaefer,Frost,Howe,House,McIntosh,Ho,Pennington,Reilly,Hebert,McFarland,Hickman,Noble,Spears,Conrad,Arias,Galvan,Velazquez,Huynh,Frederick,Randolph,Cantu,Fitzpatrick,Mahoney,Peck,Villa,Michael,Donovan,McConnell,Walls,Boyle,Mayer,Zuniga,Giles,Pineda,Pace,Hurley,Mays,McMillan,Crosby,Ayers,Case,Bentley,Shepard,Everett,Pugh,David,McMahon,Dunlap,Bender,Hahn,Harding,Acevedo,Raymond,Blackburn,Duffy,Landry,Dougherty,Bautista,Shah,Potts,Arroyo,Valentine,Meza,Gould,Vaughan,Fry,Rush,Avery,Herring,Dodson,Clements,Sampson,Tapia,Bean,Lynn,Crane,Farley,Cisneros,Benton,Ashley,McKay,Finley,Best,Blevins,Friedman,Moses,Sosa,Blanchard,Huber,Frye,Krueger,Bernard,Rosario,Rubio,Mullen,Benjamin,Haley,Chung,Moyer,Choi,Horne,Yu,Woodward,Ali,Nixon,Hayden,Rivers,Estes,Mccarty,Richmond,Stuart,Maynard,Brandt,Oconnell,Hanna,Sanford,Sheppard,Church,Burch,Levy,Rasmussen,Coffey,Ponce,Faulkner,Donaldson,Schmitt,Novak,Costa,Montes,Booker,Cordova,Waller,Arellano,Maddox,Mata,Bonilla,Stanton,Compton,Kaufman,Dudley,McPherson,Beltran,Dickson,McCann,Villegas,Proctor,Hester,Cantrell,Daugherty,Cherry,Bray,Davila,Rowland,Madden,Levine,Spence,Good,Irwin,Werner,Krause,Petty,Whitney,Baird,Hooper,Pollard,Zavala,Jarvis,Holden,Haas,Hendrix,McGrath,Bird,Lucero,Terrell,Riggs,Joyce,Mercer,Rollins,Galloway,Duke,Odom,Andersen,Downs,Hatfield,Benitez,Archer,Huerta,Travis,McNeil,Hinton,Zhang,Hays,Mayo,Fritz,Branch,Mooney,Ewing,Ritter,Esparza,Frey,Braun,Gay,Riddle,Haney,Kaiser,Holder,Chaney,McKnight,Gamble,Vang,Cooley,Carney,Cowan,Forbes,Ferrell,Davies,Barajas,Shea,Osborn,Bright,Cuevas,Bolton,Murillo,Lutz,Duarte,Kidd,Key,Cooke';

-- Organizations array
SET @organizations = 'Tech Solutions Inc,Global Innovations Ltd,Dynamic Systems Corp,Elite Enterprises,Smart Solutions Group,Advanced Technologies,Premier Services,Innovative Systems,Strategic Solutions,Quality Systems Inc,Digital Dynamics,Future Technologies,Precision Engineering,Elite Software Solutions,Global Systems Corp,Advanced Dynamics,Smart Technologies Inc,Premier Innovations,Strategic Technologies,Quality Enterprises Ltd,Digital Solutions Group,Future Systems Inc,Precision Technologies,Elite Engineering,Global Innovations Corp,Advanced Systems Ltd,Smart Enterprises,Premier Technologies,Strategic Dynamics,Quality Solutions Group';

-- Email domains array
SET @email_domains = 'gmail.com,yahoo.com,hotmail.com,outlook.com,company.com,business.net,corporate.org,enterprise.com';

-- Convert arrays to tables
DROP TEMPORARY TABLE IF EXISTS first_names;
CREATE TEMPORARY TABLE first_names (name VARCHAR(50));
SET @sql = CONCAT('INSERT INTO first_names VALUES ("', REPLACE(@first_names, ',', '"),("'), '")');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

DROP TEMPORARY TABLE IF EXISTS last_names;
CREATE TEMPORARY TABLE last_names (name VARCHAR(50));
SET @sql = CONCAT('INSERT INTO last_names VALUES ("', REPLACE(@last_names, ',', '"),("'), '")');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

DROP TEMPORARY TABLE IF EXISTS organizations;
CREATE TEMPORARY TABLE organizations (name VARCHAR(100));
SET @sql = CONCAT('INSERT INTO organizations VALUES ("', REPLACE(@organizations, ',', '"),("'), '")');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

DROP TEMPORARY TABLE IF EXISTS email_domains;
CREATE TEMPORARY TABLE email_domains (domain VARCHAR(50));
SET @sql = CONCAT('INSERT INTO email_domains VALUES ("', REPLACE(@email_domains, ',', '"),("'), '")');
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

-- Generate 5000 random contacts
DROP PROCEDURE IF EXISTS generate_contacts;
DELIMITER //
CREATE PROCEDURE generate_contacts()
BEGIN
    DECLARE i INT DEFAULT 0;
    DECLARE fn VARCHAR(50);
    DECLARE ln VARCHAR(50);
    DECLARE org VARCHAR(100);
    DECLARE domain VARCHAR(50);
    DECLARE contact_id VARCHAR(50);
    
    WHILE i < 5000 DO
        -- Get random names and organization
        SELECT name INTO fn FROM first_names ORDER BY RAND() LIMIT 1;
        SELECT name INTO ln FROM last_names ORDER BY RAND() LIMIT 1;
        SELECT name INTO org FROM organizations ORDER BY RAND() LIMIT 1;
        SELECT domain INTO domain FROM email_domains ORDER BY RAND() LIMIT 1;
        
        SET contact_id = CONCAT('C', LPAD(i, 5, '0'));
        
        -- Insert contact
        INSERT INTO contacts (
            contact_id,
            contact_type,
            organization_name,
            address_line1,
            address_line2,
            address_line3,
            postal_code,
            city,
            country,
            email,
            phone_number,
            website_url,
            tax_id,
            tax_rate,
            created_at,
            updated_at
        ) VALUES (
            contact_id,
            IF(RAND() < 0.7, 'CUSTOMER', 'SUPPLIER'),
            org,
            CONCAT(FLOOR(100 + RAND() * 9900), ' ', ELT(FLOOR(1 + RAND() * 3), 'Main St', 'Oak Ave', 'Maple Dr')),
            NULL,
            NULL,
            CONCAT(FLOOR(10000 + RAND() * 90000)),
            ELT(FLOOR(1 + RAND() * 5), 'New York', 'Los Angeles', 'Chicago', 'Houston', 'Phoenix'),
            'USA',
            LOWER(CONCAT(fn, '.', ln, '@', domain)),
            CONCAT('+1', FLOOR(1000000000 + RAND() * 9000000000)),
            CONCAT('www.', LOWER(REPLACE(org, ' ', '')), '.com'),
            CONCAT('TAX', LPAD(i, 6, '0')),
            ROUND(RAND() * 10, 2),
            NOW(),
            NOW()
        );
        
        SET i = i + 1;
    END WHILE;
END //
DELIMITER ;

-- Execute the procedure
CALL generate_contacts();

-- Clean up
DROP TEMPORARY TABLE first_names;
DROP TEMPORARY TABLE last_names;
DROP TEMPORARY TABLE organizations;
DROP TEMPORARY TABLE email_domains;
DROP PROCEDURE generate_contacts; 