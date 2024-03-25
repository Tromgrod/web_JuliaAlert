CREATE TABLE [dbo].[SupplyTextileUnit]
(
	[SupplyTextileUnitId] BIGINT PRIMARY KEY IDENTITY,
	[SupplyTextileId] BIGINT NOT NULL,
	[TextileColorId] BIGINT NOT NULL,
	[Count] DECIMAL(8, 2) NOT NULL,
	[Price] DECIMAL(8, 2) NOT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)