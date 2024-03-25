CREATE PROCEDURE [dbo].[SpecificProduct_PopulateById]
	@SpecificProductId bigint
AS
	SELECT sp.*, ps.[Name] ProductSizeName
	FROM SpecificProduct sp
	JOIN ProductSize ps ON ps.ProductSizeId = sp.ProductSizeId
	WHERE sp.DeletedBy IS NULL
	AND sp.SpecificProductId = @SpecificProductId