﻿CREATE TABLE [dbo].[Stock]
(
	[StockId] BIGINT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(100) UNIQUE NOT NULL,
	[CurrencyId] BIGINT NOT NULL,
	[IsMainStock] BIT NOT NULL DEFAULT 0,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)