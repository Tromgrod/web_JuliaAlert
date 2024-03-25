CREATE TABLE [dbo].[ImplementSupplySpecificProductUnit]
(
	[ImplementSupplySpecificProductUnitId] BIGINT PRIMARY KEY IDENTITY,
	[SupplySpecificProductUnitId] BIGINT NOT NULL,
	[Date] DATETIME NOT NULL,
	[Count] INT NOT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)