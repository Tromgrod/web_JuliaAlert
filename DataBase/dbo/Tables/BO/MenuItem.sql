CREATE TABLE [dbo].[MenuItem]
(
	[MenuItemId] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(100) NOT NULL, 
    [MenuTypeId] BIGINT NOT NULL, 
    [MenuGroupId] BIGINT NOT NULL, 
    [PageId] BIGINT NULL, 
    [Object] NVARCHAR(100) NULL, 
    [Namespace] NVARCHAR(300) NULL, 
    [Permission] BIGINT NULL, 
    [Visible] BIT NOT NULL, 
    [SortOrder] INT NOT NULL, 
    [DeletedBy] BIGINT NULL, 
    [CreatedBy] BIGINT NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateUpdated] DATETIME NULL,
    [DateCancel]  DATETIME NULL, 
    [UpdatedBy] BIGINT NULL
)
