CREATE PROCEDURE [dbo].[GetByUniqueProductAndSize]
	@UniqueProductId bigint,
	@ProductSizeId bigint
AS
    SELECT sp.*,
    cp.ColorProductId, cp.[Name] AS ColorProductName, cp.Code AS ColorProductCode,
    ps.[Name] ProductSizeName
    FROM SpecificProduct sp
    JOIN UniqueProduct up ON up.UniqueProductId = sp.UniqueProductId AND up.DeletedBy IS NULL
    JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId
    JOIN ProductSize ps ON ps.ProductSizeId = sp.ProductSizeId
    WHERE sp.DeletedBy IS NULL
    AND sp.UniqueProductId = @UniqueProductId
    AND sp.ProductSizeId = @ProductSizeId