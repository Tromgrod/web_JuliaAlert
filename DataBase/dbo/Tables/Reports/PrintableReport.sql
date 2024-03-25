CREATE TABLE [dbo].[PrintableReport]
(
	[PrintableReportId] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NOT NULL, 
	[PrintableReportTypeId] BIGINT NULL,
	[LaboratoryId] BIGINT NULL,
	[ReportTemplate] nvarchar(MAX) NULL,
	[WordTemplate] nvarchar(MAX) NULL,
    [Enabled] BIT NOT NULL,
    [DeletedBy] BIGINT NULL, 
    [CreatedBy] BIGINT NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateUpdated] DATETIME NULL
)
