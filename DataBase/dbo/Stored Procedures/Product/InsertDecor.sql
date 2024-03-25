CREATE PROCEDURE [dbo].[InsertDecor]
	@InsertDecor bigint,
	@ProductId bigint,
	@CurrentUser bigint
AS
INSERT UniqueProduct (ProductId, ColorProductId, DecorId, CreatedBy, DateCreated)
SELECT DISTINCT	ProductId, ColorProductId, @InsertDecor, @CurrentUser, GETDATE()
FROM UniqueProduct
WHERE ProductId = @ProductId AND DeletedBy IS NULL
