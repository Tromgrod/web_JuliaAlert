CREATE PROCEDURE [dbo].[Populate_ProductForOrder_CountByYear]
	@Year int,
	@SalesChannelId bigint = null
AS
	SELECT SUM(pfo.[Count]) - ISNULL(SUM(r.ReturnCount), 0) AS [Count], o.OrderDate 
	FROM [Order] o
	JOIN ProductForOrder pfo ON pfo.OrderId = o.OrderId AND pfo.DeletedBy IS NULL
	CROSS APPLY (
		SELECT SUM(r.ReturnCount) ReturnCount 
		FROM [Return] r 
		WHERE r.DeletedBy IS NULL
		AND r.ProductForOrderId = pfo.ProductForOrderId
	) r
	JOIN SalesChannel sc ON sc.SalesChannelId = o.SalesChannelId AND (@SalesChannelId IS NULL OR sc.SalesChannelId = @SalesChannelId)
	WHERE o.DeletedBy IS NULL
	AND YEAR(o.OrderDate) = @Year
	GROUP BY OrderDate