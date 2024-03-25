CREATE PROCEDURE [dbo].[UniqueProduct_GetLastPriceConsumables]
	@UniqueProductId bigint
AS
	DECLARE @FindingPrice decimal(8, 2) = (
		SELECT SUM(cf.Consumption * ISNULL(last_sf.Price, 0))
		FROM СonsumptionFinding cf
		JOIN FindingColor fc ON fc.FindingColorId = cf.FindingColorId
		JOIN Finding f ON f.FindingId = fc.FindingId
		LEFT JOIN (
			SELECT TOP 1 WITH TIES f.FindingId, sfu.Price
			FROM Finding f
			JOIN FindingColor fc ON fc.FindingId = f.FindingId
			LEFT JOIN SupplyFindingUnit sfu ON sfu.FindingColorId = fc.FindingColorId
			LEFT JOIN SupplyFinding sf ON sf.SupplyFindingId = sfu.SupplyFindingId
			ORDER BY ROW_NUMBER() OVER(PARTITION BY f.FindingId ORDER BY sf.[Date] DESC)
		) last_sf ON last_sf.FindingId = f.FindingId
		WHERE cf.DeletedBy IS NULL AND cf.UniqueProductId = @UniqueProductId
	)

	DECLARE @TextilePrice decimal(8, 2) = (
		SELECT SUM(ct.Consumption * ISNULL(last_st.Price, 0))
		FROM СonsumptionTextile ct
		JOIN TextileColor tc ON tc.TextileColorId = ct.TextileColorId
		JOIN Textile t ON t.TextileId = tc.TextileId
		LEFT JOIN (
			SELECT TOP 1 WITH TIES t.TextileId, stu.Price
			FROM Textile t
			JOIN TextileColor tc ON tc.TextileId = t.TextileId
			LEFT JOIN SupplyTextileUnit stu ON stu.TextileColorId = tc.TextileColorId
			LEFT JOIN SupplyTextile st ON st.SupplyTextileId = stu.SupplyTextileId
			ORDER BY ROW_NUMBER() OVER(PARTITION BY t.TextileId ORDER BY st.[Date] DESC)
		) last_st ON last_st.TextileId = t.TextileId
		WHERE ct.DeletedBy IS NULL AND ct.UniqueProductId = @UniqueProductId
	)

	SELECT @FindingPrice + @TextilePrice