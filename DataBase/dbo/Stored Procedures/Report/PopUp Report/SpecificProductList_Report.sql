CREATE PROCEDURE [dbo].[SpecificProductList_Report]
	@UniqueProductId BigInt,
	@StockIds NVarChar(100),
	@ProductSizeId BigInt = NULL,
	@StockId BigInt = NULL
AS
	SELECT up.UniqueProductId,
	(p.Code + '-' + cp.Code + '-' + d.Code) Code,
	ps.[Name] ProductSizeName,
	SUM(ISNULL(sps.CurrentCount, 0)) CurrentCount
	FROM SpecificProduct sp
	JOIN UniqueProduct up ON up.UniqueProductId = sp.UniqueProductId AND up.DeletedBy IS NULL
	JOIN Product p ON p.ProductId = up.ProductId AND p.DeletedBy IS NULL
	JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId AND cp.DeletedBy IS NULL
	JOIN Decor d ON d.DecorId = up.DecorId AND d.DeletedBy IS NULL
	JOIN ProductSize ps ON ps.ProductSizeId = sp.ProductSizeId AND ps.DeletedBy IS NULL
	LEFT JOIN SpecificProductStock sps ON sps.SpecificProductId = sp.SpecificProductId AND sps.DeletedBy IS NULL
	WHERE sp.DeletedBy IS NULL
	AND sp.UniqueProductId = @UniqueProductId
	AND (sps.SpecificProductStockId IS NULL OR sps.StockId IN (SELECT TRY_CONVERT(BIGINT, RTRIM(value)) FROM STRING_SPLIT(@StockIds, ',') WHERE RTRIM(value) != ''))
	AND (@StockId IS NULL OR sps.SpecificProductStockId IS NULL OR sps.StockId = @StockId)
	AND (@ProductSizeId IS NULL OR sp.ProductSizeId = @ProductSizeId)
	GROUP BY up.UniqueProductId, p.Code, cp.Code, d.Code, ps.[Name]