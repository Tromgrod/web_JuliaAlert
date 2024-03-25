CREATE TABLE [dbo].[ReturnSupplyFindingUnit]
(
	[ReturnSupplyFindingUnitId] BIGINT PRIMARY KEY IDENTITY,
	[SupplyFindingUnitId] BIGINT NOT NULL,
	[ReturnDate] DATETIME NOT NULL,
	[ReturnCount] DECIMAL(8, 2) NOT NULL,
	[CauseReturn] NVARCHAR(MAX) NOT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)