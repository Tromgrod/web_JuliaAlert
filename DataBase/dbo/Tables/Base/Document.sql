CREATE TABLE [dbo].[Document]
(
	[DocumentId] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVarCHAR(250) NOT NULL, 
    [FileName] NVarCHAR(250) NULL, 
    [Ext] NVarCHAR(4) NULL ,
    [DeletedBy]   BIGINT         NULL,
    [CreatedBy]   BIGINT         NULL,
    [DateCreated] DATETIME       NULL,
    [DateUpdated] DATETIME       NULL
)
