CREATE PROCEDURE [dbo].[UniqueProduct_GetSizes]
	@UniqueProductId bigint
AS
	SELECT ps.ProductSizeId, ps.[Name]
	FROM UniqueProduct up 
	JOIN SpecificProduct sp ON sp.UniqueProductId = up.UniqueProductId
	JOIN ProductSize ps ON ps.ProductSizeId = sp.ProductSizeId AND ps.DeletedBy IS NULL 
	WHERE up.DeletedBy IS NULL 
	AND up.UniqueProductId = @UniqueProductId