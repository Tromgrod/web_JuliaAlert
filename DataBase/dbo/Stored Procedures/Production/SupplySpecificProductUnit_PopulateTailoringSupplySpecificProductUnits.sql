CREATE PROCEDURE [dbo].[SupplySpecificProductUnit_PopulateTailoringSupplySpecificProductUnits]
	@SupplySpecificProductUnitId bigint
AS
	SELECT tsspu.*, 
	sspu.SpecificProductId, sspu.[Count] SupplySpecificProductUnitCount, 
	ssp.SupplySpecificProductId, ssp.[Date] SupplySpecificProductDate
	FROM TailoringSupplySpecificProductUnit tsspu
	JOIN SupplySpecificProductUnit sspu ON sspu.SupplySpecificProductUnitId = tsspu.SupplySpecificProductUnitId AND sspu.DeletedBy IS NULL
	JOIN SupplySpecificProduct ssp ON ssp.SupplySpecificProductId = sspu.SupplySpecificProductId AND ssp.DeletedBy IS NULL
	WHERE tsspu.SupplySpecificProductUnitId = @SupplySpecificProductUnitId
	AND tsspu.DeletedBy IS NULL