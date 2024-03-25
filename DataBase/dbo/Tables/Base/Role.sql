CREATE TABLE [dbo].[Role]
(
	[RoleId] BIGINT NOT NULL PRIMARY KEY IDENTITY,
    [Name] NVARCHAR(100) NOT NULL, 	
    [Permission] BIGINT NOT NULL, 
    [RoleAccessPermission] BIGINT NULL, 
    [AvatarId] BIGINT NULL,
    [Color] VARCHAR(7) UNIQUE NOT NULL,
    [DeletedBy] BIGINT NULL, 
    [CreatedBy] BIGINT NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateUpdated] DATETIME NULL

)
