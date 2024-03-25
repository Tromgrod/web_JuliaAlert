CREATE VIEW [dbo].[SpecificProduct_ModelList]
AS
	SELECT sp.SpecificProductId, up.UniqueProductId, up.ProductId, up.ColorProductId, up.DecorId,
	sp.ProductSizeId,
	o.OrderDate,
	sc.CurrencyId,
	pfo.[Count] AS ProductOrderCount,
	pfo.FinalPrice * pfo.[Count] AS OrderSUM,
	ISNULL(r.ReturnCount, 0) AS ReturnCount, 
	pfo.FinalPrice * ISNULL(r.ReturnCount, 0) AS ReturnSUM
	FROM SpecificProduct sp
	JOIN UniqueProduct up ON up.UniqueProductId = sp.UniqueProductId AND up.DeletedBy IS NULL
	JOIN ProductForOrder pfo ON pfo.SpecificProductId = sp.SpecificProductId AND pfo.DeletedBy IS NULL
	JOIN [Order] o ON o.OrderId = pfo.OrderId AND o.DeletedBy IS NULL
	JOIN SalesChannel sc ON sc.SalesChannelId = o.SalesChannelId
	CROSS APPLY(
		SELECT SUM(r.ReturnCount) AS ReturnCount 
		FROM [Return] r 
		WHERE r.ProductForOrderId = pfo.ProductForOrderId AND r.DeletedBy IS NULL
	) r