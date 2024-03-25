CREATE PROCEDURE [dbo].[SupplyFinding_LoadList]
AS
	SELECT TOP(20) sf.SupplyFindingId, sf.[Date], sf.DocumentNumber,
	SUM(sfu.Price) TotalPrice,
	p.ProviderId, p.[Name] ProviderName
	FROM SupplyFinding sf
	JOIN SupplyFindingUnit sfu ON sfu.SupplyFindingId = sf.SupplyFindingId
	JOIN [Provider] p ON p.ProviderId = sf.ProviderId
	GROUP BY sf.SupplyFindingId, sf.[Date], sf.DocumentNumber, p.ProviderId, p.[Name]
	ORDER BY sf.[Date] DESC