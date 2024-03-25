CREATE PROCEDURE [dbo].[SpecificProduct_SetByFullCode]
	@ProductCode int
AS
	SELECT
	sp.SpecificProductId,
	up.UniqueProductId,
	p.ProductId, p.[Name] ProductName, CAST(p.Code AS INT) ProductCode,
	cp.ColorProductId, cp.[Name] ColorProductName, cp.Code ColorProductCode, 
	d.DecorId, d.Code DecorCode,
	tp.TypeProductId, tp.[Name] TypeProductName,
	ps.ProductSizeId, ps.[Name] ProductSizeName
	FROM SpecificProduct sp 
	JOIN UniqueProduct up ON up.UniqueProductId = sp.UniqueProductId AND sp.DeletedBy IS NULL
	JOIN Product p ON p.ProductId = up.ProductId AND p.DeletedBy IS NULL
	JOIN TypeProduct tp ON tp.TypeProductId = p.TypeProductId AND tp.DeletedBy IS NULL
	JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId AND cp.DeletedBy IS NULL
	JOIN Decor d ON d.DecorId = up.DecorId AND d.DeletedBy IS NULL
	JOIN ProductSize ps ON ps.ProductSizeId = sp.ProductSizeId AND ps.DeletedBy IS NULL
	WHERE sp.ProductCode = @ProductCode
	AND sp.DeletedBy IS NULL