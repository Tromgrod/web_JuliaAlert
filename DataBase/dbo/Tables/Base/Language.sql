CREATE TABLE [dbo].[Language]
(
	[LanguageId] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [ShortName] NVARCHAR(10) NOT NULL, 
    [Culture] NVARCHAR(10) NULL, 
    [FullName] NVARCHAR(50) NOT NULL, 
    [ImageId] BIGINT NULL,
    [Enabled] BIT,
    [DeletedBy]   BIGINT         NULL,
    [CreatedBy]   BIGINT         NULL,
    [DateCreated] DATETIME       NULL,
    [DateUpdated] DATETIME       NULL
)
