CREATE TABLE [dbo].[PrintableReportType]
(
	[PrintableReportTypeId] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Tag] NVARCHAR(50) NULL, 
	[LaboratoryId] BIGINT NULL,
    [Enabled] BIT NOT NULL,
    [DeletedBy] BIGINT NULL, 
    [CreatedBy] BIGINT NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateUpdated] DATETIME NULL
)
