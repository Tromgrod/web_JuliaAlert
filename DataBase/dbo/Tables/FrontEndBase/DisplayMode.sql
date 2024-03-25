CREATE TABLE [dbo].[DisplayMode]
(
	[DisplayModeId] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Value] BIGINT NOT NULL, 
    [Name] NVARCHAR(50) NOT NULL, 
    [DeletedBy] BIGINT NULL, 
    [CreatedBy] BIGINT NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateUpdated] DATETIME NULL
)
