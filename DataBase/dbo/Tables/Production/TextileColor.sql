CREATE TABLE [dbo].[TextileColor]
(
	[TextileColorId] BIGINT PRIMARY KEY IDENTITY,
	[TextileId] BIGINT NOT NULL,
	[ColorProductId] BIGINT NOT NULL,
	[CurrentCount] DECIMAL(8, 2) NOT NULL DEFAULT 0,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL,
	UNIQUE (TextileId, ColorProductId),
	FOREIGN KEY (TextileId) REFERENCES [Textile] (TextileId) ON DELETE CASCADE,
)