CREATE PROCEDURE [dbo].[Populate_Textile]
	@TextileId bigint
AS
	SELECT t.TextileId, t.Code, t.CompoundId, t.[Name], t.ImageId,
	c.[Name] CompoundName,
	g.BOName ImageBOName, g.Ext ImageExt, g.[Name] ImageName,
	last_st.Price LastPrice,
	SUM(tc.CurrentCount) TotalCount
	FROM Textile t
	JOIN Compound c ON c.CompoundId = t.CompoundId
	JOIN TextileColor tc ON tc.TextileId = t.TextileId
	JOIN ColorProduct cl ON cl.ColorProductId = tc.ColorProductId
	LEFT JOIN SupplyTextileUnit stu ON stu.TextileColorId = tc.TextileColorId
	LEFT JOIN ReturnSupplyTextileUnit rstu ON rstu.SupplyTextileUnitId = stu.SupplyTextileUnitId
	LEFT JOIN Graphic g ON g.GraphicId = t.ImageId
	LEFT JOIN (
		SELECT TOP 1 WITH TIES t.TextileId, stu.Price
		FROM Textile t
		JOIN TextileColor tc ON tc.TextileId = t.TextileId
		LEFT JOIN SupplyTextileUnit stu ON stu.TextileColorId = tc.TextileColorId
		LEFT JOIN SupplyTextile st ON st.SupplyTextileId = stu.SupplyTextileId
		ORDER BY ROW_NUMBER() OVER(PARTITION BY t.TextileId ORDER BY st.[Date] DESC)
	) last_st ON last_st.TextileId = t.TextileId
	WHERE t.TextileId = @TextileId
	GROUP BY t.TextileId, t.Code, t.CompoundId, t.[Name], t.ImageId, c.[Name], g.BOName, g.Ext, g.[Name], last_st.Price

	SELECT cl.* FROM TextileColor tc
	JOIN ColorProduct cl ON cl.ColorProductId = tc.ColorProductId
	WHERE tc.TextileId = @TextileId