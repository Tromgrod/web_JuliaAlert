CREATE TABLE [dbo].[ProductionExpense]
(
	[ProductionExpenseId] BIGINT PRIMARY KEY IDENTITY,
	[Expense] DECIMAL(10, 2) NOT NULL DEFAULT 0,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)