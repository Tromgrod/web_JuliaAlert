CREATE PROCEDURE [dbo].[Populate_Return_CountByYear]
	@Year int,
	@SalesChannelId bigint = null
AS
	SELECT SUM(r.ReturnCount) AS ReturnCount, r.ReturnDate
	FROM [Return] r
	JOIN ProductForOrder pfo ON pfo.ProductForOrderId = r.ProductForOrderId AND pfo.DeletedBy IS NULL
	JOIN [Order] o ON o.OrderId = pfo.OrderId AND o.DeletedBy IS NULL
	JOIN SalesChannel sc ON sc.SalesChannelId = o.SalesChannelId AND (@SalesChannelId IS NULL OR sc.SalesChannelId = @SalesChannelId) AND sc.DeletedBy IS NULL
	WHERE r.DeletedBy IS NULL
	AND YEAR(r.ReturnDate) = @Year
	GROUP BY ReturnDate