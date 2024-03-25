CREATE PROCEDURE [dbo].[UniqueProduct_Insert]
	@ProductId bigint,
	@ColorProductId bigint,
	@DecorId bigint,
	@CurrentUserId bigint,
	@CompoundId bigint = null
AS
	INSERT UniqueProduct (ProductId, ColorProductId, DecorId, CompoundId, CreatedBy, DateCreated)
	SELECT TOP 1 @ProductId, @ColorProductId, @DecorId, @CompoundId, @CurrentUserId, GETDATE()
	FROM UniqueProduct
	WHERE NOT EXISTS (SELECT UniqueProductId FROM UniqueProduct WHERE ProductId = @ProductId AND ColorProductId = @ColorProductId AND DecorId = @DecorId)

	SELECT SCOPE_IDENTITY()