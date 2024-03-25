CREATE PROCEDURE [dbo].[MovingProduct_PopulateById]
	@MovingProductId bigint
AS
	SELECT *
	FROM MovingProduct mp
	WHERE mp.MovingProductId = @MovingProductId
	AND mp.DeletedBy IS NULL