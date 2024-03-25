CREATE PROCEDURE [dbo].[TailoringSupplySpecificProductUnit_PopulateById]
	@TailoringSupplySpecificProductUnitId bigint
AS
	SELECT tsspu.*,
	sspu.SpecificProductId
	FROM TailoringSupplySpecificProductUnit tsspu
	JOIN SupplySpecificProductUnit sspu ON sspu.SupplySpecificProductUnitId = tsspu.SupplySpecificProductUnitId
	WHERE tsspu.DeletedBy IS NULL 
	AND tsspu.TailoringSupplySpecificProductUnitId = @TailoringSupplySpecificProductUnitId