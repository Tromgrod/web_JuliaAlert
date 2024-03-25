CREATE PROCEDURE [dbo].[SupplySpecificProductUnit_PopulateImplementSupplySpecificProductUnits]
	@SupplySpecificProductUnitId bigint
AS
	SELECT isspu.*, 
	sspu.SpecificProductId, sspu.[Count] SupplySpecificProductUnitCount, 
	ssp.SupplySpecificProductId, ssp.[Date] SupplySpecificProductDate
	FROM ImplementSupplySpecificProductUnit isspu
	JOIN SupplySpecificProductUnit sspu ON sspu.SupplySpecificProductUnitId = isspu.SupplySpecificProductUnitId AND sspu.DeletedBy IS NULL
	JOIN SupplySpecificProduct ssp ON ssp.SupplySpecificProductId = sspu.SupplySpecificProductId AND ssp.DeletedBy IS NULL
	WHERE isspu.SupplySpecificProductUnitId = @SupplySpecificProductUnitId
	AND isspu.DeletedBy IS NULL