CREATE PROCEDURE [dbo].[Order_Populate_Latests]
	@Currencies NvarChar(100)
AS
	SELECT TOP 20 o.OrderId, o.OrderNumber, o.OrderDate, o.OrderStateId, o.CreatedBy AS CreatedById,
	sc.SalesChannelId,
	c.CurrencyId, c.[Name] AS CurrencyName,
	os.Color AS OrderStateColor,
	cl.ClientId, cl.[Name] AS ClientName,
	SUM(pfo.FinalPrice * pfo.[Count] + ISNULL(o.TAX + o.Delivery, 0) - ISNULL(pfo.FinalPrice * r.ReturnCount, 0)) AS Price
	FROM [Order] o
	JOIN OrderState os ON os.OrderStateId = o.OrderStateId
	JOIN Client cl ON cl.ClientId = o.ClientId AND cl.DeletedBy IS NULL
	LEFT JOIN ProductForOrder pfo ON pfo.OrderId = o.OrderId AND pfo.DeletedBy IS NULL
	JOIN SalesChannel sc ON sc.SalesChannelId = o.SalesChannelId
	JOIN Currency c ON c.CurrencyId = sc.CurrencyId
	CROSS APPLY (
		SELECT SUM(r.ReturnCount) ReturnCount 
		FROM [Return] r 
		WHERE r.ProductForOrderId = pfo.ProductForOrderId AND r.DeletedBy IS NULL
	) r
	WHERE o.DeletedBy IS NULL 
	AND sc.CurrencyId IN (SELECT TRY_CONVERT(BIGINT, RTRIM(value)) FROM STRING_SPLIT(@Currencies, ',') WHERE RTRIM(value) != '') 
	GROUP BY o.OrderId, o.OrderNumber, o.OrderStateId, os.Color, o.OrderDate, o.CreatedBy, sc.SalesChannelId, c.CurrencyId, c.[Name], cl.ClientId, cl.[Name]
	ORDER BY o.OrderDate DESC