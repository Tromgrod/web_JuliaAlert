CREATE PROCEDURE [dbo].[ModelList_Report]
	@CurrencyIds NVarChar(100),
	@SortColumn NVarChar(100),
	@SortType NVarChar(4),
	@UniqueProductId BigInt = NULL,
	@ProductId BigInt = NULL,
	@TypeProductId BigInt = NULL,
	@GroupProductId BigInt = NULL,
	@ProductOrderCountFrom Int = NULL,
	@ProductOrderCountTo Int = NULL,
	@OrderSUMFrom Decimal(10, 2) = NULL,
	@OrderSUMTo Decimal(10, 2) = NULL,
	@ReturnCountFrom Int = NULL,
	@ReturnCountTo Int = NULL,
	@ReturnSUMFrom Decimal(10, 2) = NULL,
	@ReturnSUMTo Decimal(10, 2) = NULL,
	@ReturnPercentFrom Decimal(10, 2) = NULL,
	@ReturnPercentTo Decimal(10, 2) = NULL,
	@SalesCountFrom Int = NULL,
	@SalesCountTo Int = NULL,
	@SalesSUMFrom Decimal(10, 2) = NULL,
	@SalesSUMTo Decimal(10, 2) = NULL,
	@OrderDateFrom DateTime = NULL,
	@OrderDateTo DateTime = NULL
AS
	SELECT * FROM (
		SELECT t.*,
		SUM(t.OrderSUM - t.ReturnSUM) AS SalesSUM, +
		SUM(t.ProductOrderCount - t.ReturnCount) AS SalesCount,
		SUM(CASE WHEN t.ProductOrderCount = 0 THEN 0 ELSE CAST(t.ReturnCount AS decimal) / t.ProductOrderCount * 100 END) AS ReturnPercent
		FROM (
			SELECT up.UniqueProductId,
			p.[Name] + ' ' + cp.[Name] + ' ' + d.[Name] AS ProductName,
			ISNULL(tp.[Name], '') AS TypeProductName,
			p.Code + '-' + cp.Code + '-' + d.Code AS Code,
			SUM(pfo.[Count]) AS ProductOrderCount,
			SUM(pfo.FinalPrice * pfo.[Count]) AS OrderSUM,
			SUM(ISNULL(r.ReturnCount, 0)) AS ReturnCount, 
			SUM(pfo.FinalPrice * ISNULL(r.ReturnCount, 0)) AS ReturnSUM
			FROM UniqueProduct up
			JOIN SpecificProduct sp ON up.UniqueProductId = sp.UniqueProductId AND up.DeletedBy IS NULL
			JOIN Product p ON p.ProductId = up.ProductId AND p.DeletedBy IS NULL
			LEFT JOIN TypeProduct tp ON tp.TypeProductId = p.TypeProductId
			JOIN Decor d ON d.DecorId = up.DecorId AND d.DeletedBy IS NULL
			JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId AND cp.DeletedBy IS NULL
			JOIN ProductForOrder pfo ON pfo.SpecificProductId = sp.SpecificProductId AND pfo.DeletedBy IS NULL
			JOIN [Order] o ON o.OrderId = pfo.OrderId AND o.DeletedBy IS NULL
			JOIN SalesChannel sc ON sc.SalesChannelId = o.SalesChannelId AND sc.DeletedBy IS NULL
			OUTER APPLY(
				SELECT SUM(r.ReturnCount) AS ReturnCount 
				FROM [Return] r 
				WHERE r.ProductForOrderId = pfo.ProductForOrderId AND r.DeletedBy IS NULL
			) r
			WHERE up.DeletedBy IS NULL
			AND (sc.CurrencyId IN (SELECT TRY_CONVERT(BIGINT, RTRIM(value)) FROM STRING_SPLIT(@CurrencyIds, ',') WHERE RTRIM(value) != ''))
			AND (@UniqueProductId IS NULL OR up.UniqueProductId = @UniqueProductId)
			AND (@ProductId IS NULL OR p.ProductId = @ProductId)
			AND (@TypeProductId IS NULL OR tp.TypeProductId = @TypeProductId)
			AND (@GroupProductId IS NULL OR tp.GroupProductId = @GroupProductId)
			AND (
				(@OrderDateFrom IS NULL AND @OrderDateTo IS NULL) 
				OR (@OrderDateFrom IS NULL AND o.OrderDate <= @OrderDateTo)
				OR (@OrderDateTo IS NULL AND o.OrderDate >= @OrderDateFrom)
				OR (o.OrderDate BETWEEN @OrderDateFrom AND @OrderDateTo)
			)
			GROUP BY up.UniqueProductId, p.[Name], p.Code, cp.[Name], cp.Code, d.[Name], d.Code, tp.[Name]
		) t
		GROUP BY t.UniqueProductId, t.ProductName, t.Code, t.TypeProductName, t.ProductOrderCount, t.OrderSUM, t.ReturnCount, t.ReturnSUM
	) t
	WHERE (
		(@ProductOrderCountFrom IS NULL AND @ProductOrderCountTo IS NULL) 
		OR (@ProductOrderCountFrom IS NULL AND t.ProductOrderCount <= @ProductOrderCountTo)
		OR (@ProductOrderCountTo IS NULL AND t.ProductOrderCount >= @ProductOrderCountFrom)
		OR (t.ProductOrderCount BETWEEN @ProductOrderCountFrom AND @ProductOrderCountTo)
	)
	AND (
		(@OrderSUMFrom IS NULL AND @OrderSUMTo IS NULL) 
		OR (@OrderSUMFrom IS NULL AND t.OrderSUM <= @OrderSUMTo)
		OR (@OrderSUMTo IS NULL AND t.OrderSUM >= @OrderSUMFrom)
		OR (t.OrderSUM BETWEEN @OrderSUMFrom AND @OrderSUMTo)
	)
	AND (
		(@ReturnCountFrom IS NULL AND @ReturnCountTo IS NULL) 
		OR (@ReturnCountFrom IS NULL AND t.ReturnCount <= @ReturnCountTo)
		OR (@ReturnCountTo IS NULL AND t.ReturnCount >= @ReturnCountFrom)
		OR (t.ReturnCount BETWEEN @ReturnCountFrom AND @ReturnCountTo)
	)
	AND (
		(@ReturnSUMFrom IS NULL AND @ReturnSUMTo IS NULL) 
		OR (@ReturnSUMFrom IS NULL AND t.ReturnSUM <= @ReturnSUMTo)
		OR (@ReturnSUMTo IS NULL AND t.ReturnSUM >= @ReturnSUMFrom)
		OR (t.ReturnSUM BETWEEN @ReturnSUMFrom AND @ReturnSUMTo)
	)
	AND (
		(@ReturnPercentFrom IS NULL AND @ReturnPercentTo IS NULL) 
		OR (@ReturnPercentFrom IS NULL AND t.ReturnPercent <= @ReturnPercentTo)
		OR (@ReturnPercentTo IS NULL AND t.ReturnPercent >= @ReturnPercentFrom)
		OR (t.ReturnPercent BETWEEN @ReturnPercentFrom AND @ReturnPercentTo)
	)
	AND (
		(@SalesCountFrom IS NULL AND @SalesCountTo IS NULL) 
		OR (@SalesCountFrom IS NULL AND t.SalesCount <= @SalesCountTo)
		OR (@SalesCountTo IS NULL AND t.SalesCount >= @SalesCountFrom)
		OR (t.SalesCount BETWEEN @SalesCountFrom AND @SalesCountTo)
	)
	AND (
		(@SalesSUMFrom IS NULL AND @SalesSUMTo IS NULL) 
		OR (@SalesSUMFrom IS NULL AND t.SalesSUM <= @SalesSUMTo)
		OR (@SalesSUMTo IS NULL AND t.SalesSUM >= @SalesSUMFrom)
		OR (t.SalesSUM BETWEEN @SalesSUMFrom AND @SalesSUMTo)
	)
	ORDER BY 
	CASE WHEN @SortColumn = 'ProductOrderCount' AND @SortType ='ASC' THEN ProductOrderCount END ASC,
	CASE WHEN @SortColumn = 'ProductOrderCount' AND @SortType ='DESC' THEN ProductOrderCount END DESC,
	CASE WHEN @SortColumn = 'OrderSUM' AND @SortType ='ASC' THEN OrderSUM END ASC,
	CASE WHEN @SortColumn = 'OrderSUM' AND @SortType ='DESC' THEN OrderSUM END DESC,
	CASE WHEN @SortColumn = 'ReturnCount' AND @SortType ='ASC' THEN ReturnCount END ASC,
	CASE WHEN @SortColumn = 'ReturnCount' AND @SortType ='DESC' THEN ReturnCount END DESC,
	CASE WHEN @SortColumn = 'ReturnSUM' AND @SortType ='ASC' THEN ReturnSUM END ASC,
	CASE WHEN @SortColumn = 'ReturnSUM' AND @SortType ='DESC' THEN ReturnSUM END DESC,
	CASE WHEN @SortColumn = 'ReturnPercent' AND @SortType ='ASC' THEN ReturnPercent END ASC,
	CASE WHEN @SortColumn = 'ReturnPercent' AND @SortType ='DESC' THEN ReturnPercent END DESC,
	CASE WHEN @SortColumn = 'SalesCount' AND @SortType ='ASC' THEN SalesCount END ASC,
	CASE WHEN @SortColumn = 'SalesCount' AND @SortType ='DESC' THEN SalesCount END DESC,
	CASE WHEN @SortColumn = 'SalesSUM' AND @SortType ='ASC' THEN SalesSUM END ASC,
	CASE WHEN @SortColumn = 'SalesSUM' AND @SortType ='DESC' THEN SalesSUM END DESC