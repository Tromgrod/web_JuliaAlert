CREATE PROCEDURE [dbo].[SupplySpecificProductUnit_GetTotalTailoringCount]
	@SupplySpecificProductUnitId bigint
AS
	SELECT SUM([Count]) FROM TailoringSupplySpecificProductUnit
	WHERE DeletedBy IS NULL
	AND SupplySpecificProductUnitId = @SupplySpecificProductUnitId