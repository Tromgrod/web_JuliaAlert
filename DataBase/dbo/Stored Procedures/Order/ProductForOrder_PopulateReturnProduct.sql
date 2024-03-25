CREATE PROCEDURE [dbo].[ProductForOrder_PopulateReturnProduct]
	@ProductForOrderId bigint
AS
	SELECT *
	FROM [Return] r
	WHERE r.DeletedBy IS NULL
	AND r.ProductForOrderId = @ProductForOrderId