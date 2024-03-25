CREATE PROCEDURE [dbo].[Populate_State]
	@search NVarChar(MAX) = null
AS
	SELECT * FROM [State] WHERE DeletedBy IS NULL 
	AND 
	(
		@search IS NULL 
		OR [Name] LIKE '%' + @search + '%' 
		OR ShortName LIKE '%' + @search + '%'
		OR [Name] + ' ' + ShortName LIKE '%' + @search + '%'
		OR [Name] + ' (' + ShortName + ')' LIKE '%' + @search + '%'
	)