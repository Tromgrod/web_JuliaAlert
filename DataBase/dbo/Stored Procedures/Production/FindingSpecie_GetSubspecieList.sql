CREATE PROCEDURE [dbo].[FindingSpecie_GetSubspecieList]
	@FindingSpecieId bigint
AS
	SELECT FindingSubspecieId, [Name] 
	FROM FindingSubspecie 
	WHERE DeletedBy IS NULL AND (@FindingSpecieId = 0 OR FindingSpecieId = @FindingSpecieId)