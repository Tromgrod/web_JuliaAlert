CREATE TABLE [dbo].[Return]
(
	[ReturnId] BIGINT PRIMARY KEY IDENTITY,
	[ProductForOrderId] BIGINT NOT NULL,
	[ReturnDate] DATETIME NOT NULL,
	[ReceivingReturnDate] DATETIME NULL,
	[CauseReturn] NVARCHAR(MAX) NULL,
	[ReturnCount] INT NOT NULL,
	[InCountry] BIT NOT NULL,
	[TrackingNumber] NVARCHAR(MAX) NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
	FOREIGN KEY (ProductForOrderId) 
		REFERENCES [ProductForOrder] (ProductForOrderId)
		ON DELETE CASCADE
)