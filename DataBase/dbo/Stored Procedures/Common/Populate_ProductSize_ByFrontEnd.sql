CREATE PROCEDURE [dbo].[Populate_ProductSize_ByFrontEnd] (
@ProductSizeId bigint
)
AS
SELECT * FROM ProductSize 
WHERE @ProductSizeId = ProductSizeId