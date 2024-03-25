CREATE VIEW [dbo].[SupplyTextileList]
AS
	SELECT t.*, t.TotalPrice - t.ReturnPrice FinalPrice FROM (
		SELECT st.SupplyTextileId, st.DocumentNumber, st.[Date], st.ProviderId, st.[Description], 
		SUM(ISNULL(stu.Price * stu.[Count], 0)) TotalPrice,
		SUM(ISNULL(stu.Price * rstu.ReturnCount, 0)) ReturnPrice
		FROM SupplyTextile st
		JOIN SupplyTextileUnit stu ON stu.SupplyTextileId = st.SupplyTextileId
		LEFT JOIN ReturnSupplyTextileUnit rstu ON rstu.SupplyTextileUnitId = stu.SupplyTextileUnitId
		WHERE st.DeletedBy IS NULL
		GROUP BY st.SupplyTextileId, st.DocumentNumber, st.[Date], st.ProviderId, st.[Description]
	) t