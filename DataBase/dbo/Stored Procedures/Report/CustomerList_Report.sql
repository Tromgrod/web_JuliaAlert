CREATE PROCEDURE [dbo].[CustomerList_Report]
	@CurrencyIds NVarChar(100),
	@SortColumn NVarChar(100),
	@SortType NVarChar(4),
	@ClientId BigInt = NULL,
	@SalesChannelId BigInt = NULL,
	@ProductOrderCountFrom Int = NULL,
	@ProductOrderCountTo Int = NULL,
	@OrderSUMFrom Decimal(5, 2) = NULL,
	@OrderSUMTo Decimal(5, 2) = NULL,
	@OrderPercentFrom Decimal(5, 2) = NULL,
	@OrderPercentTo Decimal(5, 2) = NULL,
	@ReturnCountFrom Int = NULL,
	@ReturnCountTo Int = NULL,
	@ReturnSUMFrom Decimal(5, 2) = NULL,
	@ReturnSUMTo Decimal(5, 2) = NULL,
	@ReturnPercentFrom Decimal(5, 2) = NULL,
	@ReturnPercentTo Decimal(5, 2) = NULL,
	@SalesCountFrom Int = NULL,
	@SalesCountTo Int = NULL,
	@SalesSUMFrom Decimal(5, 2) = NULL,
	@SalesSUMTo Decimal(5, 2) = NULL,
	@SalesPercentFrom Decimal(5, 2) = NULL,
	@SalesPercentTo Decimal(5, 2) = NULL,
	@OrderDateFrom DateTime = NULL,
	@OrderDateTo DateTime = NULL
AS
	SELECT * FROM (
		SELECT
		t.ClientId,
		t.ClientName,
		t.OrdersPerClient,
		t.UnitsPerClient,
		t.ProductOrderCount,
		t.OrderSUM,
		CAST(t.ProductOrderCount AS Float) * 100 / Total.OrderCount AS OrderPercent,
		t.ReturnCount,
		t.ReturnSUM,
		ISNULL(CAST(t.ReturnCount AS Float) * 100 / t.ProductOrderCount, 0) AS ReturnPercent,
		t.ReturnsPerClient,
		t.PurchasePerClient,
		t.ProductOrderCount - t.ReturnCount SalesCount,
		t.OrderSUM - t.ReturnSUM SalesSUM,
		ISNULL(CAST(t.ProductOrderCount - t.ReturnCount AS Float) * 100 / Total.SalesCount, 0) AS SalesPercent
		FROM (
			SELECT
			t.ClientId,
			t.ClientName,
			ISNULL(STRING_AGG(t.SalesChannelName + ': ' + CAST(t.OrderCount AS VarChar), '<br>'), '- - - - -') OrdersPerClient,
			ISNULL(STRING_AGG(t.SalesChannelName + ': ' + CAST(t.ProductOrderCount AS VarChar), '<br>'), '- - - - -') UnitsPerClient,
			ISNULL(SUM(t.ProductOrderCount), 0) ProductOrderCount,
			ISNULL(SUM(t.OrderSUM), 0) OrderSUM,
			ISNULL(SUM(t.ReturnCount), 0) ReturnCount,
			ISNULL(SUM(t.ReturnSUM), 0) ReturnSUM,
			ISNULL(STRING_AGG(t.SalesChannelName + ': ' + CAST(t.ReturnCount AS VarChar), '<br>'), '- - - - -') ReturnsPerClient,
			ISNULL(STRING_AGG(t.SalesChannelName + ': ' + CAST((t.ProductOrderCount - ISNULL(t.ReturnCount, 0)) AS VarChar), '<br>'), '- - - - -') PurchasePerClient,
			ISNULL(SUM(t.OrderSUM - ISNULL(t.ReturnSUM, 0)), 0) SalesSUM
			FROM (
				SELECT 
				cl.ClientId,
				cl.[Name] ClientName,
				sc.[Name] SalesChannelName, 
				COUNT(o.OrderId) OrderCount,
				SUM(pfo.FinalPrice * pfo.[Count] + o.TAX + o.Delivery) OrderSUM,
				SUM(pfo.[Count]) ProductOrderCount,
				SUM(r.ReturnCount) ReturnCount,
				ISNULL(SUM(pfo.FinalPrice * ISNULL(r.ReturnCount, 0)), 0) ReturnSUM
				FROM [Client] cl
				JOIN [Order] o ON o.ClientId = cl.ClientId AND o.DeletedBy IS NULL
				JOIN SalesChannel sc ON sc.SalesChannelId = o.SalesChannelId AND sc.DeletedBy IS NULL
				JOIN ProductForOrder pfo ON pfo.OrderId = o.OrderId AND pfo.DeletedBy IS NULL
				CROSS APPLY (
					SELECT SUM(r.ReturnCount) ReturnCount 
					FROM [Return] r 
					WHERE r.ProductForOrderId = pfo.ProductForOrderId AND r.DeletedBy IS NULL
				) r
				WHERE cl.DeletedBy IS NULL
				AND o.SalesChannelId = sc.SalesChannelId
				AND (@ClientId IS NULL OR cl.ClientId = @ClientId)
				AND (sc.CurrencyId IN (SELECT TRY_CONVERT(BIGINT, RTRIM(value)) FROM STRING_SPLIT(@CurrencyIds, ',') WHERE RTRIM(value) != ''))
				AND (@SalesChannelId IS NULL OR @SalesChannelId = sc.SalesChannelId)
				AND (
					(@OrderDateFrom IS NULL AND @OrderDateTo IS NULL) 
					OR (@OrderDateFrom IS NULL AND o.OrderDate <= @OrderDateTo)
					OR (@OrderDateTo IS NULL AND o.OrderDate >= @OrderDateFrom)
					OR (o.OrderDate BETWEEN @OrderDateFrom AND @OrderDateTo)
				)
				GROUP BY cl.ClientId, cl.[Name], sc.[Name]
			) t
			GROUP BY t.ClientId, t.ClientName
		) t
		CROSS APPLY (
			SELECT 
			SUM(pfo.[Count]) AS OrderCount,
			SUM(pfo.[Count] - ISNULL(r.ReturnCount, 0)) AS SalesCount
			FROM [ProductForOrder] pfo
			JOIN [Order] o ON o.OrderId = pfo.OrderId AND o.DeletedBy IS NULL
			JOIN [Client] cl ON cl.ClientId = o.ClientId AND cl.DeletedBy IS NULL
			JOIN [SalesChannel] sc ON sc.SalesChannelId = o.SalesChannelId AND sc.DeletedBy IS NULL
			CROSS APPLY (
				SELECT SUM(r.ReturnCount) ReturnCount 
				FROM [Return] r 
				WHERE r.ProductForOrderId = pfo.ProductForOrderId AND r.DeletedBy IS NULL
			) r
			WHERE pfo.DeletedBy IS NULL
			AND (sc.CurrencyId IN (SELECT TRY_CONVERT(BIGINT, RTRIM(value)) FROM STRING_SPLIT(@CurrencyIds, ',') WHERE RTRIM(value) != ''))
		) Total
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
	) t
	WHERE (
		(@OrderPercentFrom IS NULL AND @OrderPercentTo IS NULL) 
		OR (@OrderPercentFrom IS NULL AND t.OrderPercent <= @OrderPercentTo)
		OR (@OrderPercentTo IS NULL AND t.OrderPercent >= @OrderPercentFrom)
		OR (t.OrderPercent BETWEEN @OrderPercentFrom AND @OrderPercentTo)
	)
	AND (
		(@ReturnPercentFrom IS NULL AND @ReturnPercentTo IS NULL) 
		OR (@ReturnPercentFrom IS NULL AND t.ReturnPercent <= @ReturnPercentTo)
		OR (@ReturnPercentTo IS NULL AND t.ReturnPercent >= @ReturnPercentFrom)
		OR (t.ReturnPercent BETWEEN @ReturnPercentFrom AND @ReturnPercentTo)
	)
	AND (
		(@SalesPercentFrom IS NULL AND @SalesPercentTo IS NULL) 
		OR (@SalesPercentFrom IS NULL AND t.SalesPercent <= @SalesPercentTo)
		OR (@SalesPercentTo IS NULL AND t.SalesPercent >= @SalesPercentFrom)
		OR (t.SalesPercent BETWEEN @SalesPercentFrom AND @SalesPercentTo)
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
	CASE WHEN @SortColumn = 'ClientId' AND @SortType ='ASC' THEN ClientName END ASC,
	CASE WHEN @SortColumn = 'ClientId' AND @SortType ='DESC' THEN ClientName END DESC,
	CASE WHEN @SortColumn = 'ProductOrderCount' AND @SortType ='ASC' THEN ProductOrderCount END ASC,
	CASE WHEN @SortColumn = 'ProductOrderCount' AND @SortType ='DESC' THEN ProductOrderCount END DESC,
	CASE WHEN @SortColumn = 'OrderSUM' AND @SortType ='ASC' THEN OrderSUM END ASC,
	CASE WHEN @SortColumn = 'OrderSUM' AND @SortType ='DESC' THEN OrderSUM END DESC,
	CASE WHEN @SortColumn = 'OrderPercent' AND @SortType ='ASC' THEN OrderPercent END ASC,
	CASE WHEN @SortColumn = 'OrderPercent' AND @SortType ='DESC' THEN OrderPercent END DESC,
	CASE WHEN @SortColumn = 'ReturnCount' AND @SortType ='ASC' THEN ReturnCount END ASC,
	CASE WHEN @SortColumn = 'ReturnCount' AND @SortType ='DESC' THEN ReturnCount END DESC,
	CASE WHEN @SortColumn = 'ReturnSUM' AND @SortType ='ASC' THEN ReturnSUM END ASC,
	CASE WHEN @SortColumn = 'ReturnSUM' AND @SortType ='DESC' THEN ReturnSUM END DESC,
	CASE WHEN @SortColumn = 'ReturnPercent' AND @SortType ='ASC' THEN ReturnPercent END ASC,
	CASE WHEN @SortColumn = 'ReturnPercent' AND @SortType ='DESC' THEN ReturnPercent END DESC,
	CASE WHEN @SortColumn = 'SalesCount' AND @SortType ='ASC' THEN SalesCount END ASC,
	CASE WHEN @SortColumn = 'SalesCount' AND @SortType ='DESC' THEN SalesCount END DESC,
	CASE WHEN @SortColumn = 'SalesSUM' AND @SortType ='ASC' THEN SalesSUM END ASC,
	CASE WHEN @SortColumn = 'SalesSUM' AND @SortType ='DESC' THEN SalesSUM END DESC,
	CASE WHEN @SortColumn = 'SalesPercent' AND @SortType ='ASC' THEN SalesPercent END ASC,
	CASE WHEN @SortColumn = 'SalesPercent' AND @SortType ='DESC' THEN SalesPercent END DESC