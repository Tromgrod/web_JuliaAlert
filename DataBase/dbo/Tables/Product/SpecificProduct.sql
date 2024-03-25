CREATE TABLE [dbo].[SpecificProduct]
(
	[SpecificProductId] BIGINT PRIMARY KEY IDENTITY,
	[UniqueProductId] BIGINT NOT NULL,
	[ProductSizeId] BIGINT NOT NULL,
	[ProductCode] INT UNIQUE NOT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
	FOREIGN KEY (UniqueProductId) REFERENCES [UniqueProduct] (UniqueProductId) ON DELETE CASCADE,
	UNIQUE NONCLUSTERED (UniqueProductId, ProductSizeId)
)