CREATE PROCEDURE [dbo].[ReturnSupplyFindingUnit_PopulateByParent]
	@SupplyFindingUnitId bigint
AS
	SELECT *
	FROM ReturnSupplyFindingUnit
	WHERE SupplyFindingUnitId  = @SupplyFindingUnitId