CREATE TABLE [dbo].[SpecificProductStockHistory]
(
	[SpecificProductStockHistoryId] BIGINT PRIMARY KEY IDENTITY,
	[SpecificProductStockId] BIGINT NOT NULL,
	[Count] INT NOT NULL,
	[Date] DATETIME NOT NULL,
	[Class] NVARCHAR(20) NOT NULL,
	[ActionType] NVARCHAR(20) NOT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)