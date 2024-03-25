CREATE PROCEDURE [dbo].[Return_PopulateById]
	@ReturnId bigint
AS
	SELECT * FROM [Return] WHERE ReturnId = @ReturnId AND DeletedBy IS NULL