CREATE TABLE [dbo].[TailoringSupplySpecificProductUnit]
(
	[TailoringSupplySpecificProductUnitId] BIGINT PRIMARY KEY IDENTITY,
	[SupplySpecificProductUnitId] BIGINT NOT NULL,
	[Date] DATETIME NOT NULL,
	[Count] INT NOT NULL,
	[FactoryTailoringId] BIGINT NOT NULL,
	[TailoringCost] DECIMAL(8, 2) NOT NULL CHECK(TailoringCost > 0),
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)