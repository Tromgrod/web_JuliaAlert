CREATE TABLE [dbo].[MenuType]
(
	[MenuTypeId] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(100) NOT NULL,    
    [Alias] NVARCHAR(100) NOT NULL,    
    [Controller] NVARCHAR(100) NOT NULL,    
    [DeletedBy] BIGINT NULL, 
    [CreatedBy] BIGINT NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateUpdated] DATETIME NULL
)
