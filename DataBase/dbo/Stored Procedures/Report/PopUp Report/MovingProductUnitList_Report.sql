CREATE PROCEDURE [dbo].[MovingProductUnitList_Report]
	@MovingProductId BigInt,
	@UniqueProductId BigInt = NULL,
	@ProductId BigInt = NULL,
	@ProductSizeId BigInt = NULL,
	@StockFromId BigInt = NULL,
	@StockToId BigInt = NULL,
	@MovingCountFrom Int = NULL,
	@MovingCountTo Int = NULL
AS
	SELECT 
	mpu.MovingProductId,
	p.[Name] + ' ' + cp.[Name] + ' ' + d.[Name] ProductName,
	p.Code + '-' + cp.Code + '-' + d.Code Code,
	sFrom.[Name] StockFromName,
	sTo.[Name] StockToName,
	ps.[Name] ProductSizeName,
	mpu.[Count] MovingCount
	FROM MovingProductUnit mpu
	JOIN Stock sFrom ON sFrom.StockId = mpu.StockFromId AND sFrom.DeletedBy IS NULL
	JOIN Stock sTo ON sTo.StockId = mpu.StockToId AND sTo.DeletedBy IS NULL
	JOIN SpecificProduct sp ON sp.SpecificProductId = mpu.SpecificProductId AND sp.DeletedBy IS NULL
	JOIN UniqueProduct up ON up.UniqueProductId = sp.UniqueProductId AND up.DeletedBy IS NULL
	JOIN Product p ON p.ProductId = up.ProductId AND p.DeletedBy IS NULL
	JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId AND cp.DeletedBy IS NULL
	JOIN Decor d ON d.DecorId = up.DecorId AND d.DeletedBy IS NULL
	JOIN ProductSize ps ON ps.ProductSizeId = sp.ProductSizeId AND ps.DeletedBy IS NULL
	WHERE mpu.DeletedBy IS NULL
	AND mpu.MovingProductId = @MovingProductId
	AND (@ProductId IS NULL OR up.ProductId = @ProductId)
	AND (@UniqueProductId IS NULL OR up.UniqueProductId = @UniqueProductId)
	AND (@ProductSizeId IS NULL OR sp.ProductSizeId = @ProductSizeId)
	AND (@StockFromId IS NULL OR mpu.StockFromId = @StockFromId)
	AND (@StockToId IS NULL OR mpu.StockToId = @StockToId)
	AND (
		(@MovingCountFrom IS NULL AND @MovingCountTo IS NULL) 
		OR (@MovingCountFrom IS NULL AND mpu.[Count] <= @MovingCountTo)
		OR (@MovingCountTo IS NULL AND mpu.[Count] >= @MovingCountFrom)
		OR (mpu.[Count] BETWEEN @MovingCountFrom AND @MovingCountTo)
	)