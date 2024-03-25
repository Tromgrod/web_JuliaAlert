CREATE PROCEDURE [dbo].[SalesChannel_PopulateSalesChannelsFormula]
AS
	SELECT sc.*, c.[Name] CurrencyName
	FROM SalesChannel sc
	JOIN Currency c ON c.CurrencyId = sc.CurrencyId
	WHERE sc.DeletedBy IS NULL 
	AND (sc.Formula IS NOT NULL OR sc.Formula != '')