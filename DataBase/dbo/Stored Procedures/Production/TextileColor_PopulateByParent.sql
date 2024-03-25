CREATE PROCEDURE [dbo].[TextileColor_PopulateByParent]
	@TextileId bigint
AS
	SELECT tc.*,
	t.[Name] TextileName, t.Code TextileCode,
	c.CompoundId, c.[Name] CompoundName,
	cl.[Name] ColorProductName, cl.[Code] ColorProductCode
	FROM TextileColor tc
	JOIN ColorProduct cl ON cl.ColorProductId = tc.ColorProductId
	JOIN Textile t ON t.TextileId = tc.TextileId
	JOIN Compound c ON c.CompoundId = t.CompoundId
	WHERE tc.DeletedBy IS NULL AND tc.TextileId = @TextileId