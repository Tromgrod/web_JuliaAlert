CREATE VIEW [dbo].[CountryStatisticList]
AS
	SELECT
	co.CountriesId,
	pfo.[Count] AS ProductOrderCount,
	pfo.FinalPrice * pfo.[Count] + o.TAX + o.Delivery AS OrderSUM,
	ISNULL(r.ReturnCount, 0) AS ReturnCount,
	ISNULL(pfo.FinalPrice * r.ReturnCount, 0) AS ReturnSUM,
	pfo.[Count] - ISNULL(r.ReturnCount, 0) AS SalesCount,
	(pfo.FinalPrice * pfo.[Count] + o.TAX + o.Delivery) - ISNULL(pfo.FinalPrice * r.ReturnCount, 0) AS SalesSUM,
	sc.CurrencyId,
	o.OrderDate
	FROM Countries co
	JOIN Client cl ON cl.CountriesId = co.CountriesId AND co.DeletedBy IS NULL
	JOIN [Order] o ON o.ClientId = cl.ClientId AND o.DeletedBy IS NULL
	JOIN ProductForOrder pfo ON pfo.OrderId = o.OrderId AND pfo.DeletedBy IS NULL
	JOIN SalesChannel sc ON sc.SalesChannelId = o.SalesChannelId
	CROSS APPLY (
		SELECT SUM(r.ReturnCount) AS ReturnCount 
		FROM [Return] r 
		WHERE r.ProductForOrderId = pfo.ProductForOrderId AND r.DeletedBy IS NULL
	) r