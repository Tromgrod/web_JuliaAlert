CREATE PROCEDURE [dbo].[Populate_UniqueProduct_ById]
	@UniqueProductId bigint
AS
	SELECT up.*,
	p.[Name] AS ProductName, cp.[Name] AS ColorProductName, d.[Name] AS DecorName,
	cp.Code AS ColorProductCode, d.Code AS DecorCode, p.Code AS ProductCode,
	tp.TypeProductId, tp.[Name] AS TypeProductName,
	gp.GroupProductId, gp.[Name] AS GroupProductName,
	c.CompoundId, c.[Name] AS CompoundName,
	g.BOName AS ImageBOName, g.Ext AS ImageExt, g.Name AS ImageName
	FROM UniqueProduct up
	JOIN Product p ON p.ProductId = up.ProductId
	JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId
	JOIN Decor d ON d.DecorId = up.DecorId
	LEFT JOIN TypeProduct tp ON tp.TypeProductId = p.TypeProductId
	LEFT JOIN GroupProduct gp ON gp.GroupProductId = tp.GroupProductId
	LEFT JOIN Compound c ON c.CompoundId = up.CompoundId
	LEFT JOIN Graphic g ON g.GraphicId = up.ImageId
	WHERE up.DeletedBy IS NULL
	AND up.UniqueProductId = @UniqueProductId

	SELECT sp.SpecificProductId, sp.ProductCode, ps.ProductSizeId, ps.[Name] ProductSizeName
	FROM SpecificProduct sp
	JOIN ProductSize ps ON ps.ProductSizeId = sp.ProductSizeId
	WHERE sp.DeletedBy IS NULL
	AND sp.UniqueProductId = @UniqueProductId