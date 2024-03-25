CREATE TABLE [dbo].[Permission]
(
	[PermissionId] BIGINT NOT NULL PRIMARY KEY IDENTITY,
    [Name] NVARCHAR(100) NOT NULL, 	
    [Value] BIGINT NOT NULL, 
    [DeletedBy] BIGINT NULL, 
    [CreatedBy] BIGINT NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateUpdated] DATETIME NULL

)
