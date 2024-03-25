CREATE TABLE [dbo].[СonsumptionFinding]
(
	[СonsumptionFindingId] BIGINT PRIMARY KEY IDENTITY,
	[UniqueProductId] BIGINT NOT NULL,
	[FindingColorId] BIGINT NOT NULL,
	[Consumption] DECIMAL(8, 2) NOT NULL CHECK(Consumption > 0),
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL,
	UNIQUE (UniqueProductId, FindingColorId),
	FOREIGN KEY (UniqueProductId) REFERENCES UniqueProduct (UniqueProductId) ON DELETE CASCADE,
	FOREIGN KEY (FindingColorId) REFERENCES FindingColor (FindingColorId) ON DELETE CASCADE
)