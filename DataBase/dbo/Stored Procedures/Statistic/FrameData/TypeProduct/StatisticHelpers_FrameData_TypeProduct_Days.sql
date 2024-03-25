CREATE PROCEDURE [dbo].[StatisticHelpers_FrameData_TypeProduct_Days]
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
BEGIN
	SET NOCOUNT ON;

	DECLARE @Columns NVARCHAR(MAX);

	WITH CTE AS (
		SELECT CAST([value] AS BigInt) AS TypeProductId,
			   ROW_NUMBER() OVER (ORDER BY CHARINDEX(',' + CAST([value] AS NVARCHAR(MAX)) + ',', ',' + @DynamicFilterValues + ',')) AS rn
		FROM STRING_SPLIT(@DynamicFilterValues, ',')
	)
	SELECT @Columns = STRING_AGG(QUOTENAME(tp.[Name]), ', ') WITHIN GROUP (ORDER BY cte.rn)
	FROM CTE
	JOIN TypeProduct tp ON tp.TypeProductId = CTE.TypeProductId

	DECLARE @Year Int = CAST(@Years AS Int)

	DECLARE @CountingTypeQuery NVarChar(100)

	IF (@CountingType = 1)
		SET @CountingTypeQuery = 'SUM(pfo.[Count] - ISNULL(r.ReturnCount, 0))'
	ELSE IF (@CountingType = 2)
		SET @CountingTypeQuery = 'SUM(r.ReturnCount)'
	ELSE IF (@CountingType = 3)
		SET @CountingTypeQuery = 'SUM(pfo.[Count])'

	DECLARE @Query NVARCHAR(MAX) = '
	DECLARE @StartDate DATE = DATEFROMPARTS(' + CAST(@Year AS NVarChar) + ', ' + CAST(@MonthFrom AS NVarChar) + ', 1)
	DECLARE @EndDate DATE = EOMONTH(DATEFROMPARTS(' + CAST(@Year AS NVarChar) + ', ' + CAST(@MonthTo AS NVarChar) + ', 1))

	DECLARE @DateRange TABLE (DateValue DATE)

	WHILE @StartDate <= @EndDate
	BEGIN
		INSERT INTO @DateRange (DateValue) VALUES (@StartDate)
		SET @StartDate = DATEADD(DAY, 1, @StartDate)
	END

	SELECT ' + @Columns + '
	FROM (
		SELECT
			dr.DateValue,
			tp.[Name] AS TypeProductName,
			ISNULL(sales.Sales, 0) AS Sales
		FROM @DateRange dr
		CROSS JOIN STRING_SPLIT(''' + @DynamicFilterValues + ''', '','') typeProductIdStr
		JOIN TypeProduct tp ON tp.TypeProductId = CAST(typeProductIdStr.[value] AS BigInt)
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
			WHERE o.OrderDate = dr.DateValue
			AND p.TypeProductId = tp.TypeProductId
			' + CASE WHEN @SalesChannels IS NOT NULL THEN 'AND o.SalesChannelId IN (SELECT CAST([value] AS BigInt) FROM STRING_SPLIT(''' + @SalesChannels +  ''', '',''))' ELSE '' END + '
			' + CASE WHEN @Countries IS NOT NULL THEN 'AND cl.CountriesId IN (SELECT CAST([value] AS BigInt) FROM STRING_SPLIT(''' + @Countries +  ''', '',''))' ELSE '' END + '
			AND o.DeletedBy IS NULL
		) sales
	) AS SourceData
	PIVOT (
		SUM(Sales)
		FOR [TypeProductName] IN (' + @Columns + ')
	) AS PivotTable
	ORDER BY [DateValue] ASC
	'

	EXEC sp_executesql @Query
END