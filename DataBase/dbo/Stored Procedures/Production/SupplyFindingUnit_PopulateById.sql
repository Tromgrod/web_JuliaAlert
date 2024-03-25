CREATE PROCEDURE [dbo].[SupplyFindingUnit_PopulateById]
	@SupplyFindingUnitId bigint
AS
	SELECT sfu.*,
	sf.LocationStorageId
	FROM SupplyFindingUnit sfu
	JOIN SupplyFinding sf ON sf.SupplyFindingId = sfu.SupplyFindingId
	WHERE sfu.DeletedBy IS NULL
	AND sfu.SupplyFindingUnitId = @SupplyFindingUnitId