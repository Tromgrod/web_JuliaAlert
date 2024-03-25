CREATE PROCEDURE [dbo].[ImplementSupplySpecificProductUnit_PopulateById]
	@ImplementSupplySpecificProductUnitId bigint
AS
	SELECT isspu.*,
	sspu.SpecificProductId,
	ssp.SupplySpecificProductId, ssp.[Date] SupplySpecificProductDate
	FROM ImplementSupplySpecificProductUnit isspu
	JOIN SupplySpecificProductUnit sspu ON sspu.SupplySpecificProductUnitId = isspu.SupplySpecificProductUnitId AND sspu.DeletedBy IS NULL
	JOIN SupplySpecificProduct ssp ON ssp.SupplySpecificProductId = sspu.SupplySpecificProductId AND ssp.DeletedBy IS NULL
	WHERE isspu.DeletedBy IS NULL 
	AND ImplementSupplySpecificProductUnitId = @ImplementSupplySpecificProductUnitId