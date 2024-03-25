CREATE TABLE [dbo].[SalesChannelCoefficient]
(
	[SalesChannelCoefficientId] BIGINT PRIMARY KEY IDENTITY,
	[SalesChannelId] BIGINT NOT NULL,
	[Expense] DECIMAL(10, 2) NOT NULL DEFAULT 0,
	[Coefficient] DECIMAL(8, 2) NOT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)