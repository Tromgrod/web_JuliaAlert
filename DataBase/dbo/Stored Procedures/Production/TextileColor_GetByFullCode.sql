CREATE PROCEDURE [dbo].[TextileColor_GetByFullCode]
	@TextileCode NVarChar(10),
	@ColorCode NVarChar(10)
AS
	SELECT tc.*,
	t.[Name] TextileName, t.Code TextileCode,
	c.CompoundId, c.[Name] CompoundName,
	cl.[Name] ColorProductName, cl.[Code] ColorProductCode
	FROM TextileColor tc
	JOIN ColorProduct cl ON cl.ColorProductId = tc.ColorProductId
	JOIN Textile t ON t.TextileId = tc.TextileId
	JOIN Compound c ON c.CompoundId = t.CompoundId
	WHERE tc.DeletedBy IS NULL 
	AND t.Code = @TextileCode
	AND cl.Code = @ColorCode