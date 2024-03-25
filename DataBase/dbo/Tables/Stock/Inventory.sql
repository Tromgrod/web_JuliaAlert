CREATE TABLE [dbo].[Inventory]
(
	[InventoryId] BIGINT PRIMARY KEY IDENTITY,
	[Date] DATETIME NOT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)