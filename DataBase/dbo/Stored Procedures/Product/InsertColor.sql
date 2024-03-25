CREATE PROCEDURE [dbo].[InsertColor]
	@InsertColor bigint,
	@ProductId bigint,
	@CurrentUser bigint
AS
INSERT UniqueProduct (ProductId, ColorProductId, DecorId, CreatedBy, DateCreated)
SELECT DISTINCT	ProductId, @InsertColor, DecorId, @CurrentUser, GETDATE()
FROM UniqueProduct
WHERE ProductId = @ProductId AND DeletedBy IS NULL
