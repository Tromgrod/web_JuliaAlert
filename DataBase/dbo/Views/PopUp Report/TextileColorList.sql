CREATE VIEW [dbo].[TextileColorList]
AS
	SELECT DISTINCT
	tc.TextileColorId,
	t.TextileId,
	t.Code TextileCode,
	cl.ColorProductId,
	cl.Code ColorProductCode,
	c.CompoundId,
	ISNULL(last_st.Price, 0) Price,
	tc.CurrentCount
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
	WHERE tc.DeletedBy IS NULL