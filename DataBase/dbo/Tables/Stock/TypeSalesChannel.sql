CREATE TABLE [dbo].[TypeSalesChannel]
(
	[TypeSalesChannelId] BIGINT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(100) UNIQUE NOT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)