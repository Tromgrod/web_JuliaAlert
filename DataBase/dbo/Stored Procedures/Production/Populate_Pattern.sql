CREATE PROCEDURE [dbo].[Populate_Pattern]
	@PatternId bigint
AS
	SELECT p.*,
	prod.ProductId,
	tp.TypeProductId, tp.[Name] TypeProductName,
	gp.GroupProductId, gp.[Name] GroupProductName,
	g.BOName ImageBOName, g.Ext ImageExt, g.[Name] ImageName
	FROM Pattern p
	LEFT JOIN Product prod ON prod.PatternId = p.PatternId
	LEFT JOIN TypeProduct tp ON tp.TypeProductId = prod.TypeProductId
	LEFT JOIN GroupProduct gp ON gp.GroupProductId = tp.GroupProductId
	LEFT JOIN Graphic g ON g.GraphicId = p.ImageId
	WHERE p.PatternId = @PatternId