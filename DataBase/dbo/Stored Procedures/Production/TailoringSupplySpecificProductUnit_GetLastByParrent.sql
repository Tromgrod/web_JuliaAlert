CREATE PROCEDURE [dbo].[TailoringSupplySpecificProductUnit_GetLastByParrent]
	@SupplySpecificProductUnitId bigint
AS
	SELECT tsspu.*
	FROM TailoringSupplySpecificProductUnit tsspu
	WHERE tsspu.DeletedBy IS NULL
	AND tsspu.SupplySpecificProductUnitId = @SupplySpecificProductUnitId