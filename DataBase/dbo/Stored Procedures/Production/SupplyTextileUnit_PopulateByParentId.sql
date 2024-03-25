CREATE PROCEDURE [dbo].[SupplyTextileUnit_PopulateByParentId]
	@SupplyTextileId bigint
AS
	SELECT stu.*,
	t.TextileId, t.[Name] TextileName, t.Code TextileCode,
	c.CompoundId, c.[Name] CompoundName,
	cl.ColorProductId, cl.[Name] ColorProductName, cl.[Code] ColorProductCode
	FROM SupplyTextileUnit stu
	JOIN TextileColor tc ON tc.TextileColorId = stu.TextileColorId
	JOIN ColorProduct cl ON cl.ColorProductId = tc.ColorProductId
	JOIN Textile t ON t.TextileId = tc.TextileId
	JOIN Compound c ON c.CompoundId = t.CompoundId
	WHERE stu.DeletedBy IS NULL AND stu.SupplyTextileId = @SupplyTextileId
	ORDER BY stu.DateCreated DESC