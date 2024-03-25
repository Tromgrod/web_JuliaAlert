CREATE PROCEDURE [dbo].[ReturnSupplyTextileUnit_PopulateByParent]
	@SupplyTextileUnitId bigint
AS
	SELECT *
	FROM ReturnSupplyTextileUnit
	WHERE SupplyTextileUnitId  = @SupplyTextileUnitId