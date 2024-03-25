CREATE VIEW [dbo].[SupplyConsumableList]
AS
	SELECT t.*, t.TotalPrice - t.ReturnPrice FinalPrice FROM (
		SELECT sf.SupplyFindingId SupplyId, 'SupplyFinding' TypeSupplyLink, N'Приход фурнитуры' TypeSupplyName, sf.DocumentNumber,
		sf.[Date], sf.ProviderId, sf.[Description], SUM(ISNULL(sfu.Price * sfu.[Count], 0)) TotalPrice,
		SUM(ISNULL(sfu.Price * rsfu.ReturnCount, 0)) ReturnPrice,
		CASE WHEN COUNT(rsfu.ReturnSupplyFindingUnitId) > 0 THEN 1 ELSE 0 END IsReturn
		FROM SupplyFinding sf
		JOIN SupplyFindingUnit sfu ON sfu.SupplyFindingId = sf.SupplyFindingId AND sfu.DeletedBy IS NULL
		LEFT JOIN ReturnSupplyFindingUnit rsfu ON rsfu.SupplyFindingUnitId = sfu.SupplyFindingUnitId AND rsfu.DeletedBy IS NULL
		WHERE sf.DeletedBy IS NULL
		GROUP BY sf.SupplyFindingId, sf.DocumentNumber, sf.[Date], sf.LocationStorageId, sf.ProviderId, sf.[Description]
		UNION
		SELECT st.SupplyTextileId SupplyId, 'SupplyTextile' TypeSupplyLink, N'Приход ткани' TypeSupplyName, st.DocumentNumber, 
		st.[Date], st.ProviderId, st.[Description], SUM(ISNULL(stu.Price * stu.[Count], 0)) TotalPrice,
		SUM(ISNULL(stu.Price * rstu.ReturnCount, 0)) ReturnPrice,
		CASE WHEN COUNT(rstu.ReturnSupplyTextileUnitId) > 0 THEN 1 ELSE 0 END IsReturn
		FROM SupplyTextile st
		JOIN SupplyTextileUnit stu ON stu.SupplyTextileId = st.SupplyTextileId AND stu.DeletedBy IS NULL
		LEFT JOIN ReturnSupplyTextileUnit rstu ON rstu.SupplyTextileUnitId = stu.SupplyTextileUnitId AND rstu.DeletedBy IS NULL
		WHERE st.DeletedBy IS NULL
		GROUP BY st.SupplyTextileId, st.DocumentNumber, st.[Date], st.ProviderId, st.[Description]
	) t