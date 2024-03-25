CREATE PROCEDURE [dbo].[MovingProductUnit_PopulateByParentId]
	@MovingProductId bigint
AS
	SELECT mpu.MovingProductUnitId, mpu.StockFromId, mpu.StockToId, mpu.[Count],
	mp.MovingProductId, mp.[Date] MovingProductDate,
	up.UniqueProductId, p.ProductId, cp.ColorProductId, d.DecorId,
	p.[Name] AS ProductName, cp.[Name] AS ColorProductName, d.[Name] AS DecorName,
	p.Code AS ProductCode, cp.Code AS ColorProductCode, d.Code AS DecorCode,
	sp.SpecificProductId, sp.ProductCode SpecificProductProductCode,
	ps.ProductSizeId, ps.[Name] ProductSizeName,
	ISNULL(spsFrom.CurrentCount, 0) StockFromCount,
	ISNULL(spsTo.CurrentCount, 0) StockToCount
	FROM MovingProductUnit mpu
	JOIN MovingProduct mp ON mp.MovingProductId = mpu.MovingProductId AND mp.DeletedBy IS NULL
	JOIN SpecificProduct sp ON sp.SpecificProductId = mpu.SpecificProductId AND sp.DeletedBy IS NULL
	JOIN UniqueProduct up ON up.UniqueProductId = sp.UniqueProductId AND up.DeletedBy IS NULL
	JOIN Product p ON p.ProductId = up.ProductId AND p.DeletedBy IS NULL
	JOIN ProductSize ps ON ps.ProductSizeId = sp.ProductSizeId AND ps.DeletedBy IS NULL
	JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId
	JOIN Decor d ON d.DecorId = up.DecorId
	LEFT JOIN SpecificProductStock spsFrom ON spsFrom.SpecificProductId = sp.SpecificProductId AND spsFrom.StockId = mpu.StockFromId AND spsFrom.DeletedBy IS NULL
	LEFT JOIN SpecificProductStock spsTo ON spsTo.SpecificProductId = sp.SpecificProductId AND spsTo.StockId = mpu.StockToId AND spsTo.DeletedBy IS NULL
	WHERE mpu.DeletedBy IS NULL
	AND mpu.MovingProductId = @MovingProductId
	ORDER BY mpu.DateCreated DESC