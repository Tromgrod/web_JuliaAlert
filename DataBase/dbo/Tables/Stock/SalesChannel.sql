CREATE TABLE [dbo].[SalesChannel]
(
	[SalesChannelId] BIGINT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(100) UNIQUE NOT NULL,
	[TypeSalesChannelId] BIGINT NOT NULL,
	[InterestRate] DEC(5, 2) NOT NULL,
	[CurrencyId] BIGINT NOT NULL,
	[Formula] NVARCHAR(MAX) NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)