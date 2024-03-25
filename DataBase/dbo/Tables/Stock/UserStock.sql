CREATE TABLE [dbo].[UserStock]
(
	[UserStockId] BIGINT PRIMARY KEY IDENTITY,
	[UserId] BIGINT NOT NULL,
	[StockId] BIGINT NOT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL,
	UNIQUE (UserId, StockId)
)