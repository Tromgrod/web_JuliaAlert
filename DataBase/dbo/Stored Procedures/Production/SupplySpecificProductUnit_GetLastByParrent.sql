CREATE PROCEDURE [dbo].[SupplySpecificProductUnit_GetLastByParrent]
	@UniqueProductId bigint
AS
	SELECT *
	FROM SupplySpecificProductUnit sspu
	JOIN SpecificProduct sp ON sp.SpecificProductId = sspu.SpecificProductId AND sp.DeletedBy IS NULL
	WHERE sspu.DeletedBy IS NULL
	AND sp.UniqueProductId = @UniqueProductId