CREATE TABLE [dbo].[СonsumptionTextile]
(
	[СonsumptionTextileId] BIGINT PRIMARY KEY IDENTITY,
	[UniqueProductId] BIGINT NOT NULL,
	[TextileColorId] BIGINT NOT NULL,
	[Consumption] DECIMAL(8, 2) NOT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL,
	UNIQUE (UniqueProductId, TextileColorId),
	FOREIGN KEY (UniqueProductId) REFERENCES UniqueProduct (UniqueProductId) ON DELETE CASCADE,
	FOREIGN KEY (TextileColorId) REFERENCES TextileColor (TextileColorId) ON DELETE CASCADE
)