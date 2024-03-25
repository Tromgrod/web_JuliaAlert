CREATE TABLE [dbo].[Finding]
(
	[FindingId] BIGINT PRIMARY KEY IDENTITY,
	[ImageId] BIGINT NULL,
	[FindingSubspecieId] BIGINT NOT NULL UNIQUE,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)