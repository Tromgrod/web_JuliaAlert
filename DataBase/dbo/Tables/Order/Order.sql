CREATE TABLE [dbo].[Order]
(
	[OrderId] BIGINT PRIMARY KEY IDENTITY,
	[OrderNumber] NVARCHAR(100) NULL,
	[InvoiceNumber] NVARCHAR(100) NULL,
	[OrderStateId] BIGINT NOT NULL,
	[SalesChannelId] BIGINT NOT NULL,
	[DeliveryMethodId] BIGINT NULL,
	[TrackingNumber] NVARCHAR(MAX) NULL,
	[Delivery] DECIMAL(6, 2) NULL,
	[TAX] DECIMAL(6, 2) NULL,
	[OrderDate] DATETIME NOT NULL,
	[DepartureDate] DATETIME NULL,
	[ReceivingDate] DATETIME NULL,
	[ClientId] BIGINT NOT NULL,
	[Description] NVARCHAR(MAX) NULL,
	[StockId] BIGINT NOT NULL,
	[DeletedBy] BIGINT NULL,
	[CreatedBy] BIGINT NOT NULL,
	[DateCreated] DATETIME NOT NULL,
	[DateUpdated] DATETIME NULL
)