CREATE TABLE [dbo].[Pattern]
(
	[PatternId] BIGINT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(100) NOT NULL,
	[Code] NVARCHAR(10) NOT NULL,
	[ConstructorId] BIGINT NOT NULL,
	[CollectionId] BIGINT NOT NULL,
	[LocationStorageId] BIGINT NOT NULL,
	[ImageId] BIGINT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)