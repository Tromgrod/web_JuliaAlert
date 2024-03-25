CREATE PROCEDURE [dbo].[SpecificProduct_Insert]
	@UniqueProductId bigint,
	@ProductSizeId bigint,
	@ProductCode NVarChar(5),
	@CurrentUserId bigint
AS
	IF (NOT EXISTS (SELECT SpecificProductId FROM SpecificProduct WHERE UniqueProductId = @UniqueProductId AND ProductSizeId = @ProductSizeId))
		BEGIN
			INSERT SpecificProduct (UniqueProductId, ProductSizeId, ProductCode, CreatedBy, DateCreated)
			VALUES(@UniqueProductId, @ProductSizeId, @ProductCode, @CurrentUserId, GETDATE())
		END

	SELECT SCOPE_IDENTITY()