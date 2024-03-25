CREATE PROCEDURE [dbo].[СonsumptionFinding_PopulateByParentId]
	@UniqueProductId bigint
AS
	SELECT cf.*,
	fc.ColorProductId,
	f.FindingId,
	fsubs.FindingSubspecieId, fsubs.[Name] FindingSubspecieName, fsubs.Code FindingSubspecieCode,
	fs.FindingSpecieId, fs.[Name] FindingSpecieName, fs.Code FindingSpecieCode
	FROM СonsumptionFinding cf
	JOIN FindingColor fc ON fc.FindingColorId = cf.FindingColorId
	JOIN Finding f ON f.FindingId = fc.FindingId
	JOIN FindingSubspecie fsubs ON fsubs.FindingSubspecieId = f.FindingSubspecieId
	JOIN FindingSpecie fs ON fs.FindingSpecieId = fsubs.FindingSpecieId
	WHERE cf.DeletedBy IS NULL AND cf.UniqueProductId = @UniqueProductId