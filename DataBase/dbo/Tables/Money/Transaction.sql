CREATE TABLE [dbo].[Transaction]
(
	[TransactionId] BIGINT PRIMARY KEY IDENTITY,
	[TransactionNumber] NVARCHAR(100) NOT NULL, --UNIQUE
	[ClientId] BIGINT NOT NULL,
	[PayMethodId] BIGINT NOT NULL,
	[Sum] DECIMAL(10, 2) NOT NULL,
	[TransactionTime] DATETIME NOT NULL,
	[Note] NVARCHAR(MAX) NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)