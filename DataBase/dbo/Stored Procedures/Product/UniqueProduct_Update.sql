CREATE PROCEDURE [dbo].[UniqueProduct_Update]
	@UniqueProductId bigint,
	@CompoundId bigint
AS
	UPDATE UniqueProduct SET CompoundId = @CompoundId
	WHERE UniqueProductId = @UniqueProductId