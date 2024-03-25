CREATE PROCEDURE [dbo].[Populate_Countries]
	@search NVarChar(MAX) = null
AS
	SELECT * FROM Countries WHERE DeletedBy IS NULL 
	AND 
	(
		@search IS NULL 
		OR [Name] LIKE '%' + @search + '%' 
		OR ShortName LIKE '%' + @search + '%'
		OR [Name] + ' ' + ShortName LIKE '%' + @search + '%'
		OR [Name] + ' (' + ShortName + ')' LIKE '%' + @search + '%'
	)