CREATE TABLE [dbo].[UniqueProduct]
(
	[UniqueProductId] BIGINT PRIMARY KEY IDENTITY,
	[ProductId] BIGINT NOT NULL,
	[ColorProductId] BIGINT NOT NULL,
	[DecorId] BIGINT NOT NULL,
	[ImageId] BIGINT NULL,
	[CompoundId] BIGINT NULL,
	[Enabled] BIT NOT NULL DEFAULT 1,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
	FOREIGN KEY (ProductId) REFERENCES [Product] (ProductId) ON DELETE CASCADE,
	UNIQUE NONCLUSTERED (ProductId, ColorProductId, DecorId)
)