CREATE PROCEDURE [dbo].[Order_Populate_ByClient]
	@ClientId bigint
AS
	SELECT o.*, o.CreatedBy AS CreatedById,
	cl.[Index] AS ClientIndex, cl.Name AS ClientName, cl.Address AS ClientAddress,
	co.CountriesId, co.Name AS CountriesName,
	s.StateId, s.Name AS StateName, s.ShortName AS StateShortName,
	ci.CityId, ci.Name AS CityName,
	os.Name AS OrderStateName, os.Color AS OrderStateColor,
	c.CurrencyId, c.Name AS CurrencyName
	FROM [Order] o
	LEFT JOIN Client cl ON cl.ClientId = @ClientId
	LEFT JOIN Countries co ON co.CountriesId = cl.CountriesId
	LEFT JOIN [State] s ON s.StateId = cl.StateId
	LEFT JOIN City ci ON ci.CityId = cl.CityId
	JOIN OrderState os ON os.OrderStateId = o.OrderStateId
	JOIN SalesChannel sc ON sc.SalesChannelId = o.SalesChannelId
	JOIN Currency c ON c.CurrencyId = sc.CurrencyId
	WHERE o.DeletedBy IS NULL
	AND o.ClientId = cl.ClientId