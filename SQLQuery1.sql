﻿ALTER INDEX [PK__tmp_ms_x__737584F730F2A34D] ON [dbo].[Images] REBUILD PARTITION = ALL WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
DBCC SHRINKDATABASE(N'D:\USERS\MURAD\DOCUMENTS\SDB\DB\IMAGES.MDF' )
UPDATE Images SET Distance = 64;
UPDATE Images SET PHash = 0x;
UPDATE Images SET Person = '';
UPDATE Images SET Orbs = 0x;
UPDATE Images SET Sim = 0.0;
UPDATE Images SET Id = 0;
UPDATE Images SET LastId = 0;