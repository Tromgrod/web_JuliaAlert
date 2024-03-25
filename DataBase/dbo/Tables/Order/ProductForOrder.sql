CREATE TABLE [dbo].[ProductForOrder]
(
	[ProductForOrderId] BIGINT PRIMARY KEY IDENTITY,
	[OrderId] BIGINT NOT NULL,
	[SpecificProductId] BIGINT NOT NULL,
	[Count] INT NOT NULL,
	[FactoryPrice] DECIMAL(8, 2) NOT NULL,
	[Price] DECIMAL(8, 2) NOT NULL,
	[Discount] INT NOT NULL,
	[FinalPrice] DECIMAL(8, 2) NOT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)