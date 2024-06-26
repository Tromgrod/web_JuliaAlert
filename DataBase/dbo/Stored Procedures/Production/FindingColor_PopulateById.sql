﻿CREATE PROCEDURE [dbo].[FindingColor_PopulateById]
	@FindingColorId bigint
AS
	SELECT fc.FindingColorId, fc.ColorProductId, fc.FindingId,
	cl.[Name] ColorProductName, cl.[Code] ColorProductCode,
	fsubs.FindingSubspecieId, fsubs.[Name] FindingSubspecieName, fsubs.Code FindingSubspecieCode,
	fs.FindingSpecieId, fs.[Name] FindingSpecieName, fs.Code FindingSpecieCode,
	last_sf.Price LastPrice,
	SUM(fls.CurrentCount) CurrentCount
	FROM FindingColor fc
	JOIN ColorProduct cl ON cl.ColorProductId = fc.ColorProductId
	JOIN Finding f ON f.FindingId = fc.FindingId
	JOIN FindingSubspecie fsubs ON fsubs.FindingSubspecieId = f.FindingSubspecieId
	JOIN FindingSpecie fs ON fs.FindingSpecieId = fsubs.FindingSpecieId
	LEFT JOIN FindingLocationStorage fls ON fls.FindingColorId = fc.FindingColorId
	LEFT JOIN SupplyFindingUnit sfu ON sfu.FindingColorId = fc.FindingColorId
	LEFT JOIN (
		SELECT TOP 1 WITH TIES f.FindingId, sfu.Price
		FROM Finding f
		JOIN FindingColor fc ON fc.FindingId = f.FindingId
		LEFT JOIN SupplyFindingUnit sfu ON sfu.FindingColorId = fc.FindingColorId
		LEFT JOIN SupplyFinding sf ON sf.SupplyFindingId = sfu.SupplyFindingId
		ORDER BY ROW_NUMBER() OVER(PARTITION BY f.FindingId ORDER BY sf.[Date] DESC)
	) last_sf ON last_sf.FindingId = f.FindingId
	WHERE fc.DeletedBy IS NULL AND fc.FindingColorId = @FindingColorId
	GROUP BY fc.FindingColorId, fc.ColorProductId, fc.FindingId, cl.[Name], cl.[Code], fsubs.FindingSubspecieId,
	fsubs.[Name], fsubs.Code, fs.FindingSpecieId, fs.[Name], fs.Code, last_sf.Price