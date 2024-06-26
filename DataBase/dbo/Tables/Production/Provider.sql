﻿CREATE TABLE [dbo].[Provider]
(
	[ProviderId] BIGINT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(100) UNIQUE NOT NULL,
	[FiscalCode] NVARCHAR(100) UNIQUE NOT NULL,
	[PhoneNumber] NVARCHAR(100) NOT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)