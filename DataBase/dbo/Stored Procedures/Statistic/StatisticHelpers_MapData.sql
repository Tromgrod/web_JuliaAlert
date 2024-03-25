CREATE PROCEDURE [dbo].[StatisticHelpers_MapData]
AS
	SELECT c.CountriesId AS [Id], c.[Name], ISNULL(SUM(pfo.[Count] - ISNULL(r.ReturnCount, 0)), 0) AS [Sales]
	FROM [Order] o
	JOIN ProductForOrder pfo ON pfo.OrderId = o.OrderId AND pfo.DeletedBy IS NULL
	JOIN Client cl ON cl.ClientId = o.ClientId
	JOIN Countries c ON c.CountriesId = cl.CountriesId
	CROSS APPLY (
		SELECT SUM(r.ReturnCount) AS ReturnCount 
		FROM [Return] r 
		WHERE r.ProductForOrderId = pfo.ProductForOrderId
		AND r.DeletedBy IS NULL
	) r
	WHERE o.DeletedBy IS NULL
	GROUP BY c.CountriesId, c.[Name]