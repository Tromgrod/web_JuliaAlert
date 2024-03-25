CREATE PROCEDURE [dbo].[SupplySpecificProductUnit_PopulateById]
	@SupplySpecificProductUnitId bigint
AS
	SELECT sspu.*,
	ssp.[Date] SupplySpecificProductDate, ssp.DocumentNumber SupplySpecificProductDocumentNumber
	FROM SupplySpecificProductUnit sspu
	JOIN SupplySpecificProduct ssp ON ssp.SupplySpecificProductId = sspu.SupplySpecificProductId
	WHERE sspu.DeletedBy IS NULL
	AND sspu.SupplySpecificProductUnitId = @SupplySpecificProductUnitId