CREATE VIEW [dbo].[SupplyFindingList]
AS
	SELECT t.*, t.TotalPrice - t.ReturnPrice FinalPrice FROM (
		SELECT sf.SupplyFindingId, sf.DocumentNumber, sf.[Date], sf.LocationStorageId, sf.ProviderId, sf.[Description],
		SUM(ISNULL(sfu.Price * sfu.[Count], 0)) TotalPrice,
		SUM(ISNULL(sfu.Price * rsfu.ReturnCount, 0)) ReturnPrice
		FROM SupplyFinding sf
		JOIN SupplyFindingUnit sfu ON sfu.SupplyFindingId = sf.SupplyFindingId
		LEFT JOIN ReturnSupplyFindingUnit rsfu ON rsfu.SupplyFindingUnitId = sfu.SupplyFindingUnitId
		WHERE sf.DeletedBy IS NULL
		GROUP BY sf.SupplyFindingId, sf.DocumentNumber, sf.[Date], sf.LocationStorageId, sf.ProviderId, sf.[Description]
	) t