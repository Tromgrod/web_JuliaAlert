CREATE TABLE [dbo].[Gift]
(
	[GiftId] BIGINT PRIMARY KEY IDENTITY,
	[SpecificProductId] BIGINT NOT NULL,
	[OrderId] BIGINT NOT NULL,
	[Count] INT NOT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)