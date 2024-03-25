CREATE PROCEDURE [dbo].[StatisticHelpers_FrameData_Sales_Months]
	@Years NVarChar(100),
	@MonthFrom Int,
	@MonthTo Int,
	@DynamicFilterValues NVarChar(100) = NULL,
	@UniqueProducts NVarChar(100) = NULL,
	@SalesChannels NVarChar(100) = NULL,
	@TypeProducts NVarChar(100) = NULL,
	@Countries NVarChar(100) = NULL,
	@CountingType Int
AS
	SET NOCOUNT ON;

	DECLARE @CountingTypeQuery NVarChar(100)

	IF (@CountingType = 1)
		SET @CountingTypeQuery = 'ISNULL(SUM(pfo.[Count] - ISNULL(r.ReturnCount, 0)), 0)'
	ELSE IF (@CountingType = 2)
		SET @CountingTypeQuery = 'ISNULL(SUM(r.ReturnCount), 0)'
	ELSE IF (@CountingType = 3)
		SET @CountingTypeQuery = 'ISNULL(SUM(pfo.[Count]), 0)'

	DECLARE @Query NVARCHAR(MAX) = '
	DECLARE @MonthsRange TABLE ([Year] Int, [Month] Int)

	INSERT @MonthsRange
	SELECT CAST(years.[Value] AS Int) AS [year], months.[month]
	FROM
		(VALUES (1), (2), (3), (4), (5), (6), (7), (8), (9), (10), (11), (12)) months([month]),
		STRING_SPLIT(''' + @Years + N''', '','') years
	ORDER BY CAST(years.[Value] AS Int) ASC

	SELECT ISNULL(sales.Sales, 0) AS [Продажи шт]
	FROM @MonthsRange mr
	CROSS APPLY (
		SELECT ' + @CountingTypeQuery + ' AS Sales
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
		WHERE mr.[Year] = YEAR(o.OrderDate)
		AND mr.[Month] = MONTH(o.OrderDate)
		' + CASE WHEN @SalesChannels IS NOT NULL THEN 'AND o.SalesChannelId IN (SELECT CAST([value] AS BigInt) FROM STRING_SPLIT(''' + @SalesChannels +  ''', '',''))' ELSE '' END + '
		' + CASE WHEN @UniqueProducts IS NOT NULL THEN 'AND sp.UniqueProductId IN (SELECT CAST([value] AS BigInt) FROM STRING_SPLIT(''' + @UniqueProducts +  ''', '',''))' ELSE '' END + '
		' + CASE WHEN @TypeProducts IS NOT NULL THEN 'AND p.TypeProductId IN (SELECT CAST([value] AS BigInt) FROM STRING_SPLIT(''' + @TypeProducts +  ''', '',''))' ELSE '' END + '
		' + CASE WHEN @Countries IS NOT NULL THEN 'AND cl.CountriesId IN (SELECT CAST([value] AS BigInt) FROM STRING_SPLIT(''' + @Countries +  ''', '',''))' ELSE '' END + '
		AND o.DeletedBy IS NULL
	) sales
	WHERE mr.[Month] BETWEEN ' + CAST(@MonthFrom AS NVarChar) + ' AND ' + CAST(@MonthTo AS NVarChar) + '
	ORDER BY mr.[Year], mr.[Month]'

	EXEC sp_executesql @Query