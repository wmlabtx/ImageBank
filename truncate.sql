﻿--TRUNCATE TABLE Images;
--TRUNCATE TABLE Vars;
--INSERT INTO Vars (Id) VALUES (0);
--UPDATE Images SET Generation = 1;
--UPDATE Images SET Stars = 0;
--UPDATE Images SET Ratio = 0;
--UPDATE Images SET Id = 0;
UPDATE Images SET LastId = -1;
--UPDATE Images SET LastCheck = '20010101';
--UPDATE Images SET NextHash = '12345678';