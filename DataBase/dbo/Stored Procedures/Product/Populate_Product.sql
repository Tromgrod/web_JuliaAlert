CREATE PROCEDURE [dbo].[Populate_Product]
	@ProductId bigint
AS
	SELECT p.ProductId, p.[Name], p.Code,
	tp.TypeProductId, tp.[Name] AS TypeProductName,
	gp.GroupProductId, gp.[Name] AS TypeGroupProductName,
	patt.PatternId, patt.[Name] PatternName
	FROM Product p
	JOIN TypeProduct tp ON tp.TypeProductId = p.TypeProductId
	JOIN GroupProduct gp ON gp.GroupProductId = tp.GroupProductId
	LEFT JOIN Pattern patt ON patt.PatternId = p.PatternId
	WHERE @ProductId = p.ProductId

	SELECT DISTINCT cp.* FROM UniqueProduct up
	JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId
	WHERE up.ProductId = @ProductId

	SELECT DISTINCT ps.* FROM UniqueProduct up
	JOIN SpecificProduct sp ON sp.UniqueProductId = up.UniqueProductId
	JOIN ProductSize ps ON ps.ProductSizeId = sp.ProductSizeId
	WHERE up.ProductId = @ProductId

	SELECT DISTINCT d.* FROM UniqueProduct up
	JOIN Decor d ON d.DecorId = up.DecorId
	WHERE up.ProductId = @ProductId