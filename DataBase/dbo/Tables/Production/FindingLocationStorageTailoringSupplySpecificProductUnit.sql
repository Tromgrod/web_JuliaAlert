CREATE TABLE [dbo].[FindingLocationStorageTailoringSupplySpecificProductUnit]
(
	[FindingLocationStorageTailoringSupplySpecificProductUnitId] BIGINT PRIMARY KEY IDENTITY,
	[TailoringSupplySpecificProductUnitId] BIGINT NOT NULL,
	[FindingColorId] BIGINT NOT NULL,
	[LocationStorageId] BIGINT NOT NULL,
	[Consumption] DECIMAL(8, 2) NOT NULL CHECK(Consumption > 0),
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)