CREATE PROCEDURE [dbo].[Order_PopulateByClient]
	@ClientId bigint
AS
	SELECT o.OrderId, o.ClientId, o.Delivery, o.TAX, o.DeliveryMethodId, o.DepartureDate, o.[Description], o.InvoiceNumber, 
	o.OrderDate, o.OrderNumber, o.OrderStateId, o.ReceivingDate, o.SalesChannelId, o.StockId, o.TrackingNumber,
	os.[Name] OrderStateName, os.Color OrderStateColor,
	c.CurrencyId, c.[Name] CurrencyName,
	SUM(pfo.FinalPrice * pfo.[Count] + ISNULL(o.TAX + o.Delivery, 0) - ISNULL(pfo.FinalPrice * r.ReturnCount, 0)) AS Price
	FROM [Order] o
	JOIN ProductForOrder pfo ON pfo.OrderId = o.OrderId AND pfo.DeletedBy IS NULL
	JOIN OrderState os ON os.OrderStateId = o.OrderStateId
	JOIN SalesChannel sc ON sc.SalesChannelId = o.SalesChannelId
	JOIN Currency c ON c.CurrencyId = sc.CurrencyId
	CROSS APPLY (
		SELECT SUM(r.ReturnCount) ReturnCount 
		FROM [Return] r 
		WHERE r.ProductForOrderId = pfo.ProductForOrderId AND r.DeletedBy IS NULL
	) r
	WHERE o.DeletedBy IS NULL
	AND o.ClientId = @ClientId
	GROUP BY o.OrderId, o.ClientId, o.Delivery, o.TAX, o.DeliveryMethodId, o.DepartureDate, o.[Description], o.InvoiceNumber, 
	o.OrderDate, o.OrderNumber, o.OrderStateId, o.ReceivingDate, o.SalesChannelId, o.StockId, o.TrackingNumber,
	os.[Name], os.Color, c.CurrencyId, c.[Name]
	ORDER BY o.OrderDate DESC