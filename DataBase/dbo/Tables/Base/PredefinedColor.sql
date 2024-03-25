CREATE TABLE [dbo].[PredefinedColor]
(
	[PredefinedColorId] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Code] NVARCHAR(20) NOT NULL, 
    [Color] NVARCHAR(20) NOT NULL, 
    [DeletedBy] BIGINT NULL, 
    [CreatedBy] BIGINT NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateUpdated] DATETIME NULL
)
