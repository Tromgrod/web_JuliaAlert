CREATE VIEW [dbo].[ProviderList]
AS
	SELECT p.ProviderId, p.FiscalCode, p.PhoneNumber, totalSum.ReceiptSum, 0 PayoutSum
	FROM [Provider] p
	CROSS APPLY (
		SELECT SUM(t.[sum]) ReceiptSum FROM (
			SELECT SUM(ISNULL(stu.Price * stu.[Count], 0)) [sum] 
			FROM SupplyTextile st
			LEFT JOIN SupplyTextileUnit stu ON stu.SupplyTextileId = st.SupplyTextileId
			WHERE st.ProviderId = p.ProviderId AND st.DeletedBy IS NULL
			UNION
			SELECT SUM(ISNULL(sfu.Price * sfu.[Count], 0)) [sum] 
			FROM SupplyFinding sf
			LEFT JOIN SupplyFindingUnit sfu ON sfu.SupplyFindingId = sf.SupplyFindingId
			WHERE sf.ProviderId = p.ProviderId AND sf.DeletedBy IS NULL
		) t
	) totalSum
	WHERE p.DeletedBy IS NULL