CREATE TABLE [dbo].[KeyValueSetting]
(
	[KeyValueSettingId] BIGINT NOT NULL PRIMARY KEY IDENTITY,
    [DeletedBy]              BIGINT         NULL, 
    [Key] NVARCHAR(50) NOT NULL, 
    [Value] NVARCHAR(500) NOT NULL,
    [CreatedBy]              BIGINT         NOT NULL,
    [DateCreated]            DATETIME       NOT NULL,
    [DateUpdated]            DATETIME       NULL,
    [DateCancel]  DATETIME NULL, 
    [UpdatedBy] BIGINT NULL
)
