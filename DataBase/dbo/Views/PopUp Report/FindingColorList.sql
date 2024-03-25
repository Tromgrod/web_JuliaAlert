CREATE VIEW [dbo].[FindingColorList]
AS
	SELECT DISTINCT
	fc.FindingColorId, 
	fc.FindingId,
	cl.ColorProductId, 
	cl.Code ColorProductCode,
	fsubs.FindingSubspecieId, 
	fsubs.Code FindingSubspecieCode,
	fs.FindingSpecieId, 
	fs.Code FindingSpecieCode,
	ISNULL(last_sf.Price, 0) Price,
	SUM(ISNULL(fls.CurrentCount, 0)) CurrentCount
	FROM FindingColor fc
	JOIN ColorProduct cl ON cl.ColorProductId = fc.ColorProductId
	JOIN Finding f ON f.FindingId = fc.FindingId
	JOIN FindingSubspecie fsubs ON fsubs.FindingSubspecieId = f.FindingSubspecieId
	JOIN FindingSpecie fs ON fs.FindingSpecieId = fsubs.FindingSpecieId
	LEFT JOIN FindingLocationStorage fls ON fls.FindingColorId = fc.FindingColorId
	LEFT JOIN SupplyFindingUnit sfu ON sfu.FindingColorId = fc.FindingColorId
	LEFT JOIN ReturnSupplyFindingUnit rsfu ON rsfu.SupplyFindingUnitId = sfu.SupplyFindingUnitId
	LEFT JOIN (
		SELECT TOP 1 WITH TIES f.FindingId, sfu.Price
		FROM Finding f
		JOIN FindingColor fc ON fc.FindingId = f.FindingId
		LEFT JOIN SupplyFindingUnit sfu ON sfu.FindingColorId = fc.FindingColorId
		LEFT JOIN SupplyFinding sf ON sf.SupplyFindingId = sfu.SupplyFindingId
		ORDER BY ROW_NUMBER() OVER(PARTITION BY f.FindingId ORDER BY sf.[Date] DESC)
	) last_sf ON last_sf.FindingId = f.FindingId
	WHERE fc.DeletedBy IS NULL
	GROUP BY fc.FindingColorId, fc.FindingId, cl.ColorProductId, cl.Code, fsubs.FindingSubspecieId, fsubs.Code, fs.FindingSpecieId, fs.Code, last_sf.Price