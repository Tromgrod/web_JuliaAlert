CREATE PROCEDURE [dbo].[MovingProductUnit_PopulateById]
	@MovingProductUnitId bigint
AS
	SELECT mpu.*,
	sp.ProductSizeId,
	mp.[Date] MovingProductDate
	FROM MovingProductUnit mpu
	JOIN MovingProduct mp ON mp.MovingProductId = mpu.MovingProductId AND mp.DeletedBy IS NULL
	JOIN SpecificProduct sp ON sp.SpecificProductId = mpu.SpecificProductId AND sp.DeletedBy IS NULL
	WHERE mpu.MovingProductUnitId = @MovingProductUnitId
	AND mpu.DeletedBy IS NULL