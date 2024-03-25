CREATE PROCEDURE [dbo].[SupplySpecificProductUnit_GetTotalImplementCount]
	@SupplySpecificProductUnitId bigint
AS
	SELECT SUM([Count]) FROM ImplementSupplySpecificProductUnit
	WHERE DeletedBy IS NULL
	AND SupplySpecificProductUnitId = @SupplySpecificProductUnitId