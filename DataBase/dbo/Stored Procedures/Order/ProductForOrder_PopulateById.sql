CREATE PROCEDURE [dbo].[ProductForOrder_PopulateById]
	@ProductForOrderId bigint
AS
	SELECT pfo.*,
	o.OrderDate OrderOrderDate, o.StockId
	FROM [ProductForOrder] pfo
	JOIN [Order] o ON o.OrderId = pfo.OrderId
	WHERE pfo.DeletedBy IS NULL
	AND pfo.ProductForOrderId = @ProductForOrderId