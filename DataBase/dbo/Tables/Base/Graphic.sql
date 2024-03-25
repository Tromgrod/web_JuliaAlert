CREATE TABLE [dbo].[Graphic]
(
	[GraphicId] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVarCHAR(250) NOT NULL, 
    [Ext] NVarCHAR(4) NULL ,
    [BOName] NVarCHAR(100) NULL ,
    [DeletedBy]   BIGINT         NULL,
    [CreatedBy]   BIGINT         NULL,
    [DateCreated] DATETIME       NULL,
    [DateUpdated] DATETIME       NULL
)
