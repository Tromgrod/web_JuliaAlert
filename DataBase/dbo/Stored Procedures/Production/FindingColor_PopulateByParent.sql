CREATE PROCEDURE [dbo].[FindingColor_PopulateByParent]
	@FindingId bigint
AS
	SELECT fc.*, 
	cl.[Name] ColorProductName, cl.[Code] ColorProductCode,
	fsubs.FindingSubspecieId, fsubs.[Name] FindingSubspecieName, fsubs.Code FindingSubspecieCode,
	fs.FindingSpecieId, fs.[Name] FindingSpecieName, fs.Code FindingSpecieCode
	FROM FindingColor fc
	JOIN ColorProduct cl ON cl.ColorProductId = fc.ColorProductId
	JOIN Finding f ON f.FindingId = fc.FindingId
	JOIN FindingSubspecie fsubs ON fsubs.FindingSubspecieId = f.FindingSubspecieId
	JOIN FindingSpecie fs ON fs.FindingSpecieId = fsubs.FindingSpecieId
	WHERE fc.DeletedBy IS NULL AND fc.FindingId = @FindingId