CREATE TABLE [dbo].[SupplySpecificProduct]
(
	[SupplySpecificProductId] BIGINT PRIMARY KEY IDENTITY,
	[DocumentNumber] NVARCHAR(100) NOT NULL,
	[Date] DATETIME NOT NULL,
	[Description] NVARCHAR(MAX) NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)