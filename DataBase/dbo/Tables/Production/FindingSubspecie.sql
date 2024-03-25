CREATE TABLE [dbo].[FindingSubspecie]
(
	[FindingSubspecieId] BIGINT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(100) NOT NULL,
	[Code] NVARCHAR(10) NOT NULL,
	[FindingSpecieId] BIGINT NOT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL,
	UNIQUE ([Name], Code)
)