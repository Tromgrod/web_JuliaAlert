CREATE PROCEDURE [dbo].[StatisticHelpers_SalesTypeChartData]
	@Years NVarChar(100),
	@MonthFrom Int,
	@MonthTo Int,
	@UniqueProducts NVarChar(100) = NULL,
	@SalesChannels NVarChar(100) = NULL,
	@TypeProducts NVarChar(100) = NULL,
	@Countries NVarChar(100) = NULL,
	@CountingType Int
AS
	SELECT
	CASE WHEN @CountingType IN (1, 3) THEN ISNULL(SUM(pfo.[Count] - ISNULL(r.ReturnCount, 0)), 0) ELSE 0 END,
	ISNULL(SUM(r.ReturnCount), 0)
	FROM [Order] o
	JOIN ProductForOrder pfo ON pfo.OrderId = o.OrderId AND pfo.DeletedBy IS NULL
	JOIN SpecificProduct sp ON pfo.SpecificProductId = sp.SpecificProductId
	JOIN UniqueProduct up ON up.UniqueProductId = sp.UniqueProductId
	JOIN Product p ON p.ProductId = up.ProductId
	JOIN Client cl ON cl.ClientId = o.ClientId
	CROSS APPLY (
		SELECT SUM(r.ReturnCount) AS ReturnCount 
		FROM [Return] r 
		WHERE r.ProductForOrderId = pfo.ProductForOrderId
		AND r.DeletedBy IS NULL
	) r
	WHERE (@SalesChannels IS NULL OR o.SalesChannelId IN (SELECT CAST([value] AS BigInt) FROM STRING_SPLIT(@SalesChannels, ',')))
	AND (@UniqueProducts IS NULL OR sp.UniqueProductId IN (SELECT CAST([value] AS BigInt) FROM STRING_SPLIT(@UniqueProducts, ',')))
	AND (@TypeProducts IS NULL OR p.TypeProductId IN (SELECT CAST([value] AS BigInt) FROM STRING_SPLIT(@TypeProducts, ',')))
	AND (@Countries IS NULL OR cl.CountriesId IN (SELECT CAST([value] AS BigInt) FROM STRING_SPLIT(@Countries, ',')))
	AND YEAR(o.OrderDate) IN (SELECT CAST([value] AS Int) FROM STRING_SPLIT(@Years, ','))
	AND ((@MonthFrom = 1 AND @MonthTo = 12) OR MONTH(o.OrderDate) BETWEEN @MonthFrom AND @MonthTo)
	AND o.DeletedBy IS NULL