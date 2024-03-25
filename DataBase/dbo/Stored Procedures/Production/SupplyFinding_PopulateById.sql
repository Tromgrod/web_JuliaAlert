CREATE PROCEDURE [dbo].[SupplyFinding_PopulateById]
	@SupplyFindingId bigint
AS
	SELECT sf.*
	FROM SupplyFinding sf
	WHERE sf.DeletedBy IS NULL
	AND sf.SupplyFindingId = @SupplyFindingId