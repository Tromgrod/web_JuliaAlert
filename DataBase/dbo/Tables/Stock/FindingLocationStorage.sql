CREATE TABLE [dbo].[FindingLocationStorage]
(
	[FindingLocationStorageId] BIGINT PRIMARY KEY IDENTITY,
	[LocationStorageId] BIGINT NOT NULL,
	[FindingColorId] BIGINT NOT NULL,
	[CurrentCount] DECIMAL(8, 2) NOT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL,
	UNIQUE(LocationStorageId, FindingColorId)
)