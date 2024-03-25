CREATE PROCEDURE [dbo].[SupplyTextile_LoadList]
AS
	SELECT TOP(20) st.SupplyTextileId, st.[Date], st.DocumentNumber,
	SUM(stu.Price * stu.[Count]) TotalPrice,
	p.ProviderId, p.[Name] ProviderName
	FROM SupplyTextile st
	JOIN SupplyTextileUnit stu ON stu.SupplyTextileId = st.SupplyTextileId
	JOIN [Provider] p ON p.ProviderId = st.ProviderId
	GROUP BY st.SupplyTextileId, st.[Date], st.DocumentNumber, p.ProviderId, p.[Name]
	ORDER BY st.[Date] DESC