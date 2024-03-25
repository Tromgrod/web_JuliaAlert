CREATE TABLE [dbo].[InventoryUnit]
(
	[InventoryUnitId] BIGINT PRIMARY KEY IDENTITY,
	[InventoryId] BIGINT NOT NULL,
	[SpecificProductStockId] BIGINT NOT NULL,
	[CountInStock] INT NOT NULL,
	[CurrentCount] INT NOT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)