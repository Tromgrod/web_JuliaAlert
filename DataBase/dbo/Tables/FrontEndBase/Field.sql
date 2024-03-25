CREATE TABLE [dbo].[Field]
(
	[FieldId] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
	[PageId] BIGINT NOT NULL,
    [Permission] BIGINT NULL, 
	[LaboratoryId] BIGINT NOT NULL,
    [Name] NVARCHAR(500) NOT NULL, 
    [PrintName] NVARCHAR(500) NOT NULL, 
    [FieldName] NVARCHAR(300) NOT NULL, 
    [DeletedBy] BIGINT NULL, 
    [CreatedBy] BIGINT NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateUpdated] DATETIME NULL
)
