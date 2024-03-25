CREATE PROCEDURE [dbo].[UserStock_GetStocksByUser]
	@UserId bigint
AS
	SELECT s.*
	FROM Stock s
	JOIN UserStock us ON us.StockId = s.StockId AND us.UserId = @UserId AND us.DeletedBy IS NULL
	WHERE s.DeletedBy IS NULL