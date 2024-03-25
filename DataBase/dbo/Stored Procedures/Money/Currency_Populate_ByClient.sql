CREATE PROCEDURE [dbo].[Currency_Populate_ByClient]
	@ClientId bigint
AS
	SELECT c.CurrencyId, c.[Name]
	FROM Client cl
	JOIN [Order] o On o.ClientId = cl.ClientId AND o.DeletedBy IS NULL
	JOIN SalesChannel sc ON sc.SalesChannelId = o.SalesChannelId AND sc.DeletedBy IS NULL
	JOIN Currency c ON c.CurrencyId = sc.CurrencyId AND c.DeletedBy IS NULL
	WHERE cl.ClientId = @ClientId AND cl.DeletedBy IS NULL
	GROUP BY c.CurrencyId, c.[Name]