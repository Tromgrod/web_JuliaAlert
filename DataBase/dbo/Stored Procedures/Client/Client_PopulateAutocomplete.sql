CREATE PROCEDURE [dbo].[Client_PopulateAutocomplete]
	@search NVarChar(MAX),
	@param NVarChar(MAX) = NULL
AS
	SELECT DISTINCT TOP(10) cl.ClientId, cl.[Name] 
	FROM Client cl
	LEFT JOIN [Order] o ON o.ClientId = cl.ClientId
	LEFT JOIN SalesChannel sc ON sc.SalesChannelId = o.SalesChannelId
	WHERE cl.DeletedBy IS NULL
	AND (@search = '' OR cl.[Name] LIKE '%' + @search + '%' )
	AND ((@param IS NULL OR @param = '') OR sc.CurrencyId IN (SELECT TRY_CONVERT(BIGINT, RTRIM(value)) FROM STRING_SPLIT(@param, ',') WHERE RTRIM(value) != '') OR sc.CurrencyId IS NULL)
	ORDER BY cl.[Name]