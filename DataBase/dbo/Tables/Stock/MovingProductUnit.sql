CREATE TABLE [dbo].[MovingProductUnit]
(
	[MovingProductUnitId] BIGINT PRIMARY KEY IDENTITY,
	[MovingProductId] BIGINT NOT NULL,
	[StockFromId] BIGINT NULL,
	[StockToId] BIGINT NULL,
	[SpecificProductId] BIGINT NOT NULL,
	[Count] INT NOT NULL CHECK([Count] > 0),
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL,
	FOREIGN KEY (MovingProductId) REFERENCES [MovingProduct] (MovingProductId) ON DELETE CASCADE
)