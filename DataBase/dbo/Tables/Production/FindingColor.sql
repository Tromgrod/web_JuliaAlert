CREATE TABLE [dbo].[FindingColor]
(
	[FindingColorId] BIGINT PRIMARY KEY IDENTITY,
	[FindingId] BIGINT NOT NULL,
	[ColorProductId] BIGINT NOT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL,
	UNIQUE (FindingId, ColorProductId),
	FOREIGN KEY (FindingId) REFERENCES [Finding] (FindingId) ON DELETE CASCADE,
)