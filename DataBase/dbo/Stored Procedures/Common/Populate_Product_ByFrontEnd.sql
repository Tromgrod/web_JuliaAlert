CREATE PROCEDURE [dbo].[Populate_Product](
@ProductId bigint
)
AS
SELECT * FROM Product p
INNER JOIN ProductSize ps on ps.ProductSizeId = p.ProductSizeId
WHERE @ProductId = ProductId