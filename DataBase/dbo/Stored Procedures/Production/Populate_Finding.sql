CREATE PROCEDURE [dbo].[Populate_Finding]
	@FindingId bigint
AS
	SELECT f.FindingId, f.ImageId,
	fsubs.FindingSubspecieId, fsubs.[Name] FindingSubspecieName, fsubs.Code FindingSubspecieCode,
	fs.FindingSpecieId, fs.[Name] FindingSpecieName, fs.Code FindingSpecieCode,
	g.BOName ImageBOName, g.Ext ImageExt, g.[Name] ImageName,
	last_sf.Price LastPrice,
	SUM(ISNULL(fls.CurrentCount, 0)) TotalCount
	FROM Finding f
	JOIN FindingSubspecie fsubs ON fsubs.FindingSubspecieId = f.FindingSubspecieId
	JOIN FindingSpecie fs ON fs.FindingSpecieId = fsubs.FindingSpecieId
	JOIN FindingColor fc ON fc.FindingId = f.FindingId
	LEFT JOIN FindingLocationStorage fls ON fls.FindingColorId = fc.FindingColorId
	LEFT JOIN SupplyFindingUnit sfu ON sfu.FindingColorId = fc.FindingColorId
	LEFT JOIN ReturnSupplyFindingUnit rsfu ON rsfu.SupplyFindingUnitId = sfu.SupplyFindingUnitId
	LEFT JOIN Graphic g ON g.GraphicId = f.ImageId
	LEFT JOIN (
		SELECT TOP 1 WITH TIES f.FindingId, sfu.Price
		FROM Finding f
		JOIN FindingColor fc ON fc.FindingId = f.FindingId
		LEFT JOIN SupplyFindingUnit sfu ON sfu.FindingColorId = fc.FindingColorId
		LEFT JOIN SupplyFinding sf ON sf.SupplyFindingId = sfu.SupplyFindingId
		ORDER BY ROW_NUMBER() OVER(PARTITION BY f.FindingId ORDER BY sf.[Date] DESC)
	) last_sf ON last_sf.FindingId = f.FindingId
	WHERE f.FindingId = @FindingId
	GROUP BY f.FindingId, f.ImageId, fsubs.FindingSubspecieId, fsubs.[Name], fsubs.Code, fs.FindingSpecieId, fs.[Name], fs.Code, g.BOName, g.Ext, g.[Name], last_sf.Price

	SELECT cl.* FROM FindingColor fc
	JOIN ColorProduct cl ON cl.ColorProductId = fc.ColorProductId
	WHERE fc.FindingId = @FindingId