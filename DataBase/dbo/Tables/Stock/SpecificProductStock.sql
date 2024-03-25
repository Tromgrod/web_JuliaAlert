CREATE TABLE [dbo].[SpecificProductStock]
(
	[SpecificProductStockId] BIGINT PRIMARY KEY IDENTITY,
	[StockId] BIGINT NOT NULL,
	[SpecificProductId] BIGINT NOT NULL,
	[CurrentCount] INT NOT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL,
	UNIQUE (StockId, SpecificProductId)
)