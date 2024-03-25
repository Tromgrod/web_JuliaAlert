CREATE PROCEDURE [dbo].[TextileColor_PopulateById]
	@TextileColorId bigint
AS
	SELECT tc.TextileColorId, tc.TextileId, tc.TextileColorId, tc.ColorProductId, tc.CurrentCount,
	t.[Name] TextileName, t.Code TextileCode,
	c.CompoundId, c.[Name] CompoundName,
	cl.[Name] ColorProductName, cl.[Code] ColorProductCode,
	last_st.Price LastPrice
	FROM TextileColor tc
	JOIN ColorProduct cl ON cl.ColorProductId = tc.ColorProductId
	JOIN Textile t ON t.TextileId = tc.TextileId
	JOIN Compound c ON c.CompoundId = t.CompoundId
	LEFT JOIN SupplyTextileUnit stu ON stu.TextileColorId = tc.TextileColorId
	LEFT JOIN ReturnSupplyTextileUnit rstu ON rstu.SupplyTextileUnitId = stu.SupplyTextileUnitId
	LEFT JOIN (
		SELECT TOP 1 WITH TIES t.TextileId, stu.Price
		FROM Textile t
		JOIN TextileColor tc ON tc.TextileId = t.TextileId
		LEFT JOIN SupplyTextileUnit stu ON stu.TextileColorId = tc.TextileColorId
		LEFT JOIN SupplyTextile st ON st.SupplyTextileId = stu.SupplyTextileId
		ORDER BY ROW_NUMBER() OVER(PARTITION BY t.TextileId ORDER BY st.[Date] DESC)
	) last_st ON last_st.TextileId = t.TextileId
	WHERE tc.DeletedBy IS NULL AND tc.TextileColorId = @TextileColorId