CREATE PROCEDURE [dbo].[Client_Populate_One]
	@ClientId bigint
AS
	SELECT *,
	co.Name AS CountriesName, co.ShortName AS CountriesShortName,
	s.Name AS StateName, s.ShortName AS StateShortName,
	ci.Name AS CityName
	FROM Client cl
	LEFT JOIN Countries co ON co.CountriesId = cl.CountriesId
	LEFT JOIN State s ON s.StateId = cl.StateId
	LEFT JOIN City ci ON ci.CityId = cl.CityId
	WHERE cl.DeletedBy IS NULL
	AND cl.ClientId = @ClientId