CREATE PROCEDURE [dbo].[ReturnSupplyFindingUnit_PopulateById]
	@ReturnSupplyFindingUnitId bigint
AS
	SELECT rsfu.*,
	sfu.FindingColorId,
	sf.SupplyFindingId, sf.LocationStorageId
	FROM ReturnSupplyFindingUnit rsfu
	JOIN SupplyFindingUnit sfu ON sfu.SupplyFindingUnitId = rsfu.SupplyFindingUnitId
	JOIN SupplyFinding sf ON sf.SupplyFindingId = sfu.SupplyFindingId
	WHERE rsfu.DeletedBy IS NULL
	AND rsfu.ReturnSupplyFindingUnitId = @ReturnSupplyFindingUnitId