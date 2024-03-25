CREATE PROCEDURE [dbo].[Insert-Update_ProductPrice]
	@ProductId bigint,
	@SalesChannelId bigint,
	@Price decimal,
	@CurrentUser bigint
AS
	IF (NOT EXISTS(SELECT * FROM ProductPrice WHERE ProductId = @ProductId AND SalesChannelId = @SalesChannelId))
		BEGIN
			INSERT ProductPrice (ProductId, SalesChannelId, Price, CreatedBy, DateCreated)
			SELECT @ProductId, @SalesChannelId, @Price, @CurrentUser, GETDATE()
		END
	ELSE
		BEGIN
			UPDATE ProductPrice SET Price = @Price, DateUpdated = GETDATE()
			WHERE ProductId = @ProductId AND SalesChannelId = @SalesChannelId
		END