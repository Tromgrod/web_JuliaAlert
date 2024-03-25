CREATE PROCEDURE [dbo].[Finding_PopulateAutocomplete]
	@search NVarChar(MAX) = null
AS
	SELECT f.*,
	fsubs.[Name] FindingSubspecieName, fsubs.Code FindingSubspecieCode,
	fs.FindingSpecieId, fs.[Name] FindingSpecieName, fs.Code FindingSpecieCode
	FROM Finding f
	JOIN FindingSubspecie fsubs ON fsubs.FindingSubspecieId = f.FindingSubspecieId
	JOIN FindingSpecie fs ON fs.FindingSpecieId = fsubs.FindingSpecieId
	WHERE (
	@search IS NULL 
	OR fsubs.[Name] + ' ' + fs.[Name] LIKE '%' + @search
	OR fs.[Name] + ' ' + fsubs.[Name]  LIKE '%' + @search
	OR fsubs.[Name] LIKE '%' + @search + '%'
	OR fs.[Name] LIKE '%' + @search + '%'
	)