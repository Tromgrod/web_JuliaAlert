CREATE TABLE [dbo].[ProductPrice]
(
	[ProductPriceId] BIGINT PRIMARY KEY IDENTITY,
	[ProductId] BIGINT NOT NULL,
	[SalesChannelId] BIGINT NOT NULL,
	[Price] DECIMAL(7, 2) NOT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL,
	UNIQUE NONCLUSTERED (ProductId, SalesChannelId)
)