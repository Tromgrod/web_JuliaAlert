CREATE PROCEDURE [dbo].[TextileColor_DeleteByTextile]
	@TextileId bigint,
	@ColorId bigint
AS
	DELETE TextileColor
	WHERE TextileId = @TextileId AND ColorProductId = @ColorId