CREATE PROCEDURE [dbo].[FindingLocationStorageTailoringSupplySpecificProductUnit_PopulateByParentId]
	@TailoringSupplySpecificProductUnitId bigint
AS
	SELECT
	flstsspu.FindingLocationStorageTailoringSupplySpecificProductUnitId,
	tsspu.TailoringSupplySpecificProductUnitId,
	fc.FindingColorId,
	sp.SpecificProductId,
	flstsspu.Consumption,
	f.FindingId,
	fsubs.FindingSubspecieId, fsubs.[Name] FindingSubspecieName, fsubs.Code FindingSubspecieCode,
	fs.FindingSpecieId, fs.[Name] FindingSpecieName, fs.Code FindingSpecieCode,
	cp.ColorProductId, cp.[Name] ColorProductName, cp.[Code] ColorProductCode,
	ls.LocationStorageId, ls.[Name] LocationStorageName
	FROM FindingLocationStorageTailoringSupplySpecificProductUnit flstsspu
	JOIN TailoringSupplySpecificProductUnit tsspu ON tsspu.TailoringSupplySpecificProductUnitId = flstsspu.TailoringSupplySpecificProductUnitId AND tsspu.DeletedBy IS NULL
	JOIN SupplySpecificProductUnit sspu ON sspu.SupplySpecificProductUnitId = tsspu.SupplySpecificProductUnitId AND sspu.DeletedBy IS NULL
	JOIN SpecificProduct sp ON sp.SpecificProductId = sspu.SpecificProductId AND sp.DeletedBy IS NULL
	JOIN FindingColor fc ON fc.FindingColorId = flstsspu.FindingColorId AND fc.DeletedBy IS NULL
	JOIN Finding f ON f.FindingId = fc.FindingId AND f.DeletedBy IS NULL
	JOIN FindingSubspecie fsubs ON fsubs.FindingSubspecieId = f.FindingSubspecieId AND fsubs.DeletedBy IS NULL
	JOIN FindingSpecie fs ON fs.FindingSpecieId = fsubs.FindingSpecieId AND fc.DeletedBy IS NULL
	JOIN ColorProduct cp ON cp.ColorProductId = fc.ColorProductId AND cp.DeletedBy IS NULL
	JOIN LocationStorage ls ON ls.LocationStorageId = flstsspu.LocationStorageId AND ls.DeletedBy IS NULL
	WHERE flstsspu.TailoringSupplySpecificProductUnitId = @TailoringSupplySpecificProductUnitId
	AND flstsspu.DeletedBy IS NULL
	UNION
	SELECT
	0 FindingLocationStorageTailoringSupplySpecificProductUnitId,
	tsspu.TailoringSupplySpecificProductUnitId,
	fc.FindingColorId,
	sp.SpecificProductId,
	cf.Consumption,
	f.FindingId,
	fsubs.FindingSubspecieId, fsubs.[Name] FindingSubspecieName, fsubs.Code FindingSubspecieCode,
	fs.FindingSpecieId, fs.[Name] FindingSpecieName, fs.Code FindingSpecieCode,
	cp.ColorProductId, cp.[Name] ColorProductName, cp.[Code] ColorProductCode,
	0 LocationStorageId, NULL LocationStorageName
	FROM TailoringSupplySpecificProductUnit tsspu
	JOIN SupplySpecificProductUnit sspu ON sspu.SupplySpecificProductUnitId = tsspu.SupplySpecificProductUnitId AND sspu.DeletedBy IS NULL
	JOIN SpecificProduct sp ON sp.SpecificProductId = sspu.SpecificProductId AND sp.DeletedBy IS NULL
	JOIN СonsumptionFinding cf ON cf.UniqueProductId = sp.UniqueProductId AND cf.DeletedBy IS NULL
	JOIN FindingColor fc ON fc.FindingColorId = cf.FindingColorId AND fc.DeletedBy IS NULL
	JOIN Finding f ON f.FindingId = fc.FindingId AND f.DeletedBy IS NULL
	JOIN FindingSubspecie fsubs ON fsubs.FindingSubspecieId = f.FindingSubspecieId AND fsubs.DeletedBy IS NULL
	JOIN FindingSpecie fs ON fs.FindingSpecieId = fsubs.FindingSpecieId AND fc.DeletedBy IS NULL
	JOIN ColorProduct cp ON cp.ColorProductId = fc.ColorProductId AND cp.DeletedBy IS NULL
	WHERE tsspu.TailoringSupplySpecificProductUnitId = @TailoringSupplySpecificProductUnitId
	AND tsspu.DeletedBy IS NULL
	AND fc.FindingColorId NOT IN (SELECT FindingColorId FROM FindingLocationStorageTailoringSupplySpecificProductUnit WHERE TailoringSupplySpecificProductUnitId = @TailoringSupplySpecificProductUnitId)
	ORDER BY FindingColorId 