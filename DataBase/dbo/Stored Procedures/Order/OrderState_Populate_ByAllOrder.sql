CREATE PROCEDURE [dbo].[OrderState_Populate_ByAllOrder]
	@Currencies NvarChar(100)
AS	
	SELECT os.OrderStateId, os.[Name], os.Color, COUNT(o.OrderId) AS [Count]
	FROM OrderState os
	JOIN [Order] o ON o.OrderStateId = os.OrderStateId
	JOIN SalesChannel sc ON sc.SalesChannelId = o.SalesChannelId
	WHERE os.DeletedBy IS NULL
	AND o.DeletedBy IS NULL
	AND sc.CurrencyId IN (SELECT TRY_CONVERT(BIGINT, RTRIM(value)) FROM STRING_SPLIT(@Currencies, ',') WHERE RTRIM(value) != '')
	AND sc.DeletedBy IS NULL
	GROUP BY os.OrderStateId, os.[Name], os.Color