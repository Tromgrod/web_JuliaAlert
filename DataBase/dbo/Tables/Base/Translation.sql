CREATE TABLE [dbo].[Translation]
(
	[TranslationId] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Alias] NVARCHAR(50) NOT NULL, 
    [Language] NVARCHAR(10) NOT NULL, 
    [Text] NVARCHAR(MAX) NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateUpdated] DATETIME NULL
)
