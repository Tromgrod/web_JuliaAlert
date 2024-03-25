CREATE TABLE [dbo].[News]
(
	[NewsId] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Text] NVARCHAR(MAX) NOT NULL, 
    [Date] DATE NOT NULL, 
    [Title] NVARCHAR(300) NOT NULL, 
    [Active] BIT NULL, 
    [DeletedBy] BIGINT NULL, 
    [CreatedBy] BIGINT NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateUpdated] DATETIME NULL
)
