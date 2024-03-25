CREATE PROCEDURE [dbo].[OrderList_Report]
	@CurrencyIds NVarChar(100),
	@SortColumn NVarChar(100),
	@SortType NVarChar(4),
	@OrderNumber NVarChar(100) = NULL,
	@InvoiceNumber NVarChar(100) = NULL,
	@UniqueProductId BigInt = NULL,
	@ProductId BigInt = NULL,
	@TypeProductId BigInt = NULL,
	@GroupProductId BigInt = NULL,
	@OrderDateFrom DateTime = NULL,
	@OrderDateTo DateTime = NULL,
	@OrderStateId BigInt = NULL,
	@SalesChannelId BigInt = NULL,
	@ClientId BigInt = NULL,
	@Phone NVarChar(100) = NULL,
	@TotalSUMFrom Decimal(10, 2) = NULL,
	@TotalSUMTo Decimal(10, 2) = NULL,
	@ReturnSUMFrom Decimal(10, 2) = NULL,
	@ReturnSUMTo Decimal(10, 2) = NULL,
	@SalesSUMFrom Decimal(10, 2) = NULL,
	@SalesSUMTo Decimal(10, 2) = NULL,
	@RealSUMFrom Decimal(10, 2) = NULL,
	@RealSUMTo Decimal(10, 2) = NULL,
	@TotalCountFrom Int = NULL,
	@TotalCountTo Int = NULL,
	@ReturnCountFrom Int = NULL,
	@ReturnCountTo Int = NULL,
	@SalesCountFrom Int = NULL,
	@SalesCountTo Int = NULL
AS
	SELECT * FROM (
		SELECT 
		t.*,
		t.TotalSUM - t.ReturnSUM SalesSUM,
		t.TotalCount - t.ReturnCount SalesCount,
		(t.TotalSUM - t.ReturnSUM) * (1 - t.SalesChannelInterestRate / 100) RealSUM
		FROM (
			SELECT 
			o.OrderId,
			o.OrderNumber,
			o.InvoiceNumber,
			STRING_AGG(p.Code + '-' + cp.Code + '-' + d.Code, '<br>') Codes,
			o.OrderDate,
			o.[Description],
			os.[Name] OrderStateName, os.Color OrderStateColor,
			sc.[Name] SalesChannelName, sc.InterestRate SalesChannelInterestRate,
			ISNULL(c.[Name], '') ClientName,
			curr.[Name] CurrencyName,
			SUM(ISNULL(pfo.FinalPrice * pfo.[Count], 0)) + o.TAX + o.Delivery TotalSUM, 
			SUM(ISNULL(pfo.FinalPrice * r.ReturnCount, 0)) ReturnSUM,
			SUM(ISNULL(pfo.[Count], 0)) TotalCount,
			SUM(ISNULL(r.ReturnCount, 0)) ReturnCount
			FROM [Order] o
			JOIN OrderState os ON os.OrderStateId = o.OrderStateId
			JOIN SalesChannel sc ON sc.SalesChannelId = o.SalesChannelId AND sc.DeletedBy IS NULL
			JOIN Currency curr ON curr.CurrencyId = sc.CurrencyId AND curr.DeletedBy IS NULL
			LEFT JOIN Client c ON c.ClientId = o.ClientId AND c.DeletedBy IS NULL
			LEFT JOIN ProductForOrder pfo ON pfo.OrderId = o.OrderId AND pfo.DeletedBy IS NULL
			LEFT JOIN SpecificProduct sp ON sp.SpecificProductId = pfo.SpecificProductId AND sp.DeletedBy IS NULL
			LEFT JOIN UniqueProduct up ON up.UniqueProductId = sp.UniqueProductId AND up.DeletedBy IS NULL
			LEFT JOIN Product p ON p.ProductId = up.ProductId AND p.DeletedBy IS NULL
			LEFT JOIN TypeProduct tp ON tp.TypeProductId = p.TypeProductId AND tp.DeletedBy IS NULL
			LEFT JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId AND cp.DeletedBy IS NULL
			LEFT JOIN Decor d ON d.DecorId = up.DecorId AND d.DeletedBy IS NULL
			CROSS APPLY(
				SELECT SUM(r.ReturnCount) ReturnCount 
				FROM [Return] r 
				WHERE r.ProductForOrderId = pfo.ProductForOrderId AND r.DeletedBy IS NULL
			) r
			WHERE o.DeletedBy IS NULL
			AND (sc.CurrencyId IN (SELECT TRY_CONVERT(BIGINT, RTRIM(value)) FROM STRING_SPLIT(@CurrencyIds, ',') WHERE RTRIM(value) != ''))
			AND (@OrderNumber IS NULL OR o.OrderNumber LIKE '%' + @OrderNumber + '%')
			AND (@InvoiceNumber IS NULL OR o.InvoiceNumber LIKE '%' + @InvoiceNumber + '%')
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
			AND (@OrderStateId IS NULL OR os.OrderStateId = @OrderStateId)
			AND (@SalesChannelId IS NULL OR sc.SalesChannelId = @SalesChannelId)
			AND (@ClientId IS NULL OR c.ClientId = @ClientId)
			AND (@Phone IS NULL OR c.Phone LIKE '%' + @Phone + '%')
			GROUP BY o.OrderId, o.OrderNumber, o.InvoiceNumber, o.OrderDate, o.[Description], os.[Name], os.Color, sc.[Name], sc.InterestRate, c.[Name], curr.[Name], o.TAX, o.Delivery
		) t
		WHERE (
			(@TotalSUMFrom IS NULL AND @TotalSUMTo IS NULL) 
			OR (@TotalSUMFrom IS NULL AND t.TotalSUM <= @TotalSUMTo)
			OR (@TotalSUMTo IS NULL AND t.TotalSUM >= @TotalSUMFrom)
			OR (t.TotalSUM BETWEEN @TotalSUMFrom AND @TotalSUMTo)
		)
		AND (
			(@ReturnSUMFrom IS NULL AND @ReturnSUMTo IS NULL) 
			OR (@ReturnSUMFrom IS NULL AND t.ReturnSUM <= @ReturnSUMTo)
			OR (@ReturnSUMTo IS NULL AND t.ReturnSUM >= @ReturnSUMFrom)
			OR (t.ReturnSUM BETWEEN @ReturnSUMFrom AND @ReturnSUMTo)
		)
		AND (
			(@TotalCountFrom IS NULL AND @TotalCountTo IS NULL) 
			OR (@TotalCountFrom IS NULL AND t.TotalCount <= @TotalCountTo)
			OR (@TotalCountTo IS NULL AND t.TotalCount >= @TotalCountFrom)
			OR (t.TotalCount BETWEEN @TotalCountFrom AND @TotalCountTo)
		)
		AND (
			(@ReturnCountFrom IS NULL AND @ReturnCountTo IS NULL) 
			OR (@ReturnCountFrom IS NULL AND t.ReturnCount <= @ReturnCountTo)
			OR (@ReturnCountTo IS NULL AND t.ReturnCount >= @ReturnCountFrom)
			OR (t.ReturnCount BETWEEN @ReturnCountFrom AND @ReturnCountTo)
		)
	) t
	WHERE (
		(@SalesSUMFrom IS NULL AND @SalesSUMTo IS NULL) 
		OR (@SalesSUMFrom IS NULL AND t.SalesSUM <= @SalesSUMTo)
		OR (@SalesSUMTo IS NULL AND t.SalesSUM >= @SalesSUMFrom)
		OR (t.SalesSUM BETWEEN @SalesSUMFrom AND @SalesSUMTo)
	)
	AND (
		(@RealSUMFrom IS NULL AND @RealSUMTo IS NULL) 
		OR (@RealSUMFrom IS NULL AND t.RealSUM <= @RealSUMTo)
		OR (@RealSUMTo IS NULL AND t.RealSUM >= @RealSUMFrom)
		OR (t.RealSUM BETWEEN @RealSUMFrom AND @RealSUMTo)
	)
	AND (
		(@SalesCountFrom IS NULL AND @SalesCountTo IS NULL) 
		OR (@SalesCountFrom IS NULL AND t.SalesCount <= @SalesCountTo)
		OR (@SalesCountTo IS NULL AND t.SalesCount >= @SalesCountFrom)
		OR (t.SalesCount BETWEEN @SalesCountFrom AND @SalesCountTo)
	)
	ORDER BY 
	CASE WHEN @SortColumn = 'OrderDate' AND @SortType ='ASC' THEN OrderDate END ASC,
	CASE WHEN @SortColumn = 'OrderDate' AND @SortType ='DESC' THEN OrderDate END DESC,
	CASE WHEN @SortColumn = 'OrderState' AND @SortType ='ASC' THEN OrderStateName END ASC,
	CASE WHEN @SortColumn = 'OrderState' AND @SortType ='DESC' THEN OrderStateName END DESC,
	CASE WHEN @SortColumn = 'SalesChannel' AND @SortType ='ASC' THEN SalesChannelName END ASC,
	CASE WHEN @SortColumn = 'SalesChannel' AND @SortType ='DESC' THEN SalesChannelName END DESC,
	CASE WHEN @SortColumn = 'ClientId' AND @SortType ='ASC' THEN ClientName END ASC,
	CASE WHEN @SortColumn = 'ClientId' AND @SortType ='DESC' THEN ClientName END DESC,
	CASE WHEN @SortColumn = 'TotalSUM' AND @SortType ='ASC' THEN TotalSUM END ASC,
	CASE WHEN @SortColumn = 'TotalSUM' AND @SortType ='DESC' THEN TotalSUM END DESC,
	CASE WHEN @SortColumn = 'ReturnSUM' AND @SortType ='ASC' THEN ReturnSUM END ASC,
	CASE WHEN @SortColumn = 'ReturnSUM' AND @SortType ='DESC' THEN ReturnSUM END DESC,
	CASE WHEN @SortColumn = 'SalesSUM' AND @SortType ='ASC' THEN SalesSUM END ASC,
	CASE WHEN @SortColumn = 'SalesSUM' AND @SortType ='DESC' THEN SalesSUM END DESC,
	CASE WHEN @SortColumn = 'RealSUM' AND @SortType ='ASC' THEN RealSUM END ASC,
	CASE WHEN @SortColumn = 'RealSUM' AND @SortType ='DESC' THEN RealSUM END DESC,
	CASE WHEN @SortColumn = 'TotalCount' AND @SortType ='ASC' THEN TotalCount END ASC,
	CASE WHEN @SortColumn = 'TotalCount' AND @SortType ='DESC' THEN TotalCount END DESC,
	CASE WHEN @SortColumn = 'ReturnCount' AND @SortType ='ASC' THEN ReturnCount END ASC,
	CASE WHEN @SortColumn = 'ReturnCount' AND @SortType ='DESC' THEN ReturnCount END DESC,
	CASE WHEN @SortColumn = 'SalesCount' AND @SortType ='ASC' THEN SalesCount END ASC,
	CASE WHEN @SortColumn = 'SalesCount' AND @SortType ='DESC' THEN SalesCount END DESC