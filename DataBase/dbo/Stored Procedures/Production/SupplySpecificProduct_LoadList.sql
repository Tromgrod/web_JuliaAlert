CREATE PROCEDURE [dbo].[SupplySpecificProduct_LoadList]
AS
	SELECT TOP(20) ssp.SupplySpecificProductId, ssp.[Date], ssp.DocumentNumber,
	SUM(sspu.[Count]) TotalCount
	FROM SupplySpecificProduct ssp
	JOIN SupplySpecificProductUnit sspu ON sspu.SupplySpecificProductId = ssp.SupplySpecificProductId
	GROUP BY ssp.SupplySpecificProductId, ssp.[Date], ssp.DocumentNumber
	ORDER BY ssp.[Date] DESC