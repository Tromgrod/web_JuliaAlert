CREATE PROCEDURE [dbo].[Delete_SpecificProduct]
	@Product bigint,
	@DeleteSize bigint
AS
DELETE SpecificProduct
FROM UniqueProduct up
WHERE up.UniqueProductId = SpecificProduct.UniqueProductId 
AND up.ProductId = @Product
AND SpecificProduct.ProductSizeId = @DeleteSize