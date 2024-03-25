CREATE TABLE [dbo].[LocationStorage]
(
	[LocationStorageId] BIGINT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(100) NOT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)