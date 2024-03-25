CREATE PROCEDURE [dbo].[Product_Populate_Autocomplete]
	@search NVarChar(MAX) = null
AS
	SELECT ProductId, [Code] AS [Name] FROM Product WHERE DeletedBy IS NULL 
	AND 
	(
		@search IS NULL 
		OR [Name] LIKE '%' + @search + '%' 
		OR Code LIKE '%' + @search + '%'
	)