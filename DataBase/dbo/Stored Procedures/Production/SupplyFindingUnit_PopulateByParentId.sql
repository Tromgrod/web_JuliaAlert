CREATE PROCEDURE [dbo].[SupplyFindingUnit_PopulateByParentId]
	@SupplyFindingId bigint
AS
	SELECT sfu.*,
	f.FindingId,
	fsubs.FindingSubspecieId, fsubs.[Name] FindingSubspecieName, fsubs.Code FindingSubspecieCode,
	fs.FindingSpecieId, fs.[Name] FindingSpecieName, fs.Code FindingSpecieCode,
	cl.ColorProductId, cl.[Name] ColorProductName, cl.[Code] ColorProductCode
	FROM SupplyFindingUnit sfu
	JOIN FindingColor fc ON fc.FindingColorId = sfu.FindingColorId
	JOIN ColorProduct cl ON cl.ColorProductId = fc.ColorProductId
	JOIN Finding f ON f.FindingId = fc.FindingId
	JOIN FindingSubspecie fsubs ON fsubs.FindingSubspecieId = f.FindingSubspecieId
	JOIN FindingSpecie fs ON fs.FindingSpecieId = fsubs.FindingSpecieId
	WHERE sfu.DeletedBy IS NULL AND sfu.SupplyFindingId = @SupplyFindingId
	ORDER BY sfu.DateCreated DESC