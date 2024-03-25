CREATE PROCEDURE [dbo].[ProductPrice_PopulateByProduct]
	@ProductId bigint
AS
	SELECT * FROM ProductPrice WHERE DeletedBy IS NULL AND ProductId = @ProductId