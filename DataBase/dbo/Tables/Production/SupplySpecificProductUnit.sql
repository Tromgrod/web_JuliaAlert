CREATE TABLE [dbo].[SupplySpecificProductUnit]
(
	[SupplySpecificProductUnitId] BIGINT PRIMARY KEY IDENTITY,
	[SupplySpecificProductId] BIGINT NOT NULL,
	[SpecificProductId] BIGINT NOT NULL,
	[FactoryCutId] BIGINT NOT NULL,
	[CutCost] DECIMAL(8, 2) NOT NULL CHECK(CutCost > 0),
	[Count] INT NOT NULL CHECK([Count] > 0),
	[FactoryPrice] DECIMAL(8, 2) NOT NULL CHECK([FactoryPrice] > 0),
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL,
	FOREIGN KEY (SupplySpecificProductId) REFERENCES [SupplySpecificProduct] (SupplySpecificProductId) ON DELETE CASCADE
)