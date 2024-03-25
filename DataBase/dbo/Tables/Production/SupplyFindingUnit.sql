CREATE TABLE [dbo].[SupplyFindingUnit]
(
	[SupplyFindingUnitId] BIGINT PRIMARY KEY IDENTITY,
	[SupplyFindingId] BIGINT NOT NULL,
	[FindingColorId] BIGINT NOT NULL,
	[Count] DECIMAL(8, 2) NOT NULL,
	[Price] DECIMAL(8, 2) NOT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)