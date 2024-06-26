﻿CREATE PROCEDURE [dbo].[StatisticHelpers_FrameData_Sales_Years]
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

	DECLARE @Query NVARCHAR(MAX) = N'
	SELECT ISNULL(sales.Sales, 0) AS [Продажи шт]
	FROM STRING_SPLIT(''' + @Years + ''', '','') years
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
		WHERE YEAR(o.OrderDate) = CAST(years.[value] As Int)
		' + CASE WHEN @SalesChannels IS NOT NULL THEN 'AND o.SalesChannelId IN (SELECT CAST([value] AS BigInt) FROM STRING_SPLIT(''' + @SalesChannels +  ''', '',''))' ELSE '' END + '
		' + CASE WHEN @UniqueProducts IS NOT NULL THEN 'AND sp.UniqueProductId IN (SELECT CAST([value] AS BigInt) FROM STRING_SPLIT(''' + @UniqueProducts +  ''', '',''))' ELSE '' END + '
		' + CASE WHEN @TypeProducts IS NOT NULL THEN 'AND p.TypeProductId IN (SELECT CAST([value] AS BigInt) FROM STRING_SPLIT(''' + @TypeProducts +  ''', '',''))' ELSE '' END + '
		' + CASE WHEN @Countries IS NOT NULL THEN 'AND cl.CountriesId IN (SELECT CAST([value] AS BigInt) FROM STRING_SPLIT(''' + @Countries +  ''', '',''))' ELSE '' END + '
		AND o.DeletedBy IS NULL
	) sales
	ORDER BY CAST(years.[value] As Int)'

	EXEC sp_executesql @Query