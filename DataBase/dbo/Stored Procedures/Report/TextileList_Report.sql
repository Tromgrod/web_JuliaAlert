CREATE PROCEDURE [dbo].[TextileList_Report]
	@SortColumn NVarChar(100),
	@SortType NVarChar(4),
	@TextileId BigInt = NULL,
	@Code NVarChar(100) = NULL,
	@CompoundId BigInt = NULL,
	@CountFrom Decimal(10, 2) = NULL,
	@CountTo Decimal(10, 2) = NULL,
	@PriceFrom Decimal(10, 2) = NULL,
	@PriceTo Decimal(10, 2) = NULL,
	@TotalPriceFrom Decimal(10, 2) = NULL,
	@TotalPriceTo Decimal(10, 2) = NULL
AS
	SELECT t.*, Price * [CurrentCount] TotalPrice FROM ( 
		SELECT
		t.TextileId,
		t.[Name] TextileName,
		t.Code,
		c.[Name] CompoundName,
		colors.val Colors,
		ISNULL(last_st.Price, 0) Price,
		SUM(tc.CurrentCount) CurrentCount
		FROM Textile t
		JOIN TextileColor tc ON tc.TextileId = t.TextileId
		JOIN ColorProduct cl ON cl.ColorProductId = tc.ColorProductId
		JOIN Compound c ON c.CompoundId = t.CompoundId
		LEFT JOIN SupplyTextileUnit stu ON stu.TextileColorId = tc.TextileColorId
		LEFT JOIN ReturnSupplyTextileUnit rstu ON rstu.SupplyTextileUnitId = stu.SupplyTextileUnitId
		LEFT JOIN (
			SELECT TOP 1 WITH TIES t.TextileId, stu.Price
			FROM Textile t
			JOIN TextileColor tc ON tc.TextileId = t.TextileId
			LEFT JOIN SupplyTextileUnit stu ON stu.TextileColorId = tc.TextileColorId
			LEFT JOIN SupplyTextile st ON st.SupplyTextileId = stu.SupplyTextileId
			ORDER BY ROW_NUMBER() OVER(PARTITION BY t.TextileId ORDER BY st.[Date] DESC)
		) last_st ON last_st.TextileId = t.TextileId
		JOIN (
			SELECT t.TextileId, STRING_AGG(cl.[Name], ', ') val 
			FROM Textile t
			JOIN TextileColor tc ON tc.TextileId = t.TextileId
			JOIN ColorProduct cl ON cl.ColorProductId = tc.ColorProductId
			GROUP BY t.TextileId
		) colors ON colors.TextileId = t.TextileId
		WHERE t.DeletedBy IS NULL
		AND (@TextileId IS NULL OR t.TextileId = @TextileId)
		AND (@Code IS NULL OR t.Code LIKE '%' + @Code + '%')
		AND (@CompoundId IS NULL OR t.CompoundId = @CompoundId)
		GROUP BY t.TextileId, t.[Name], t.Code, c.[Name], last_st.Price, colors.val
	) t
	WHERE (
		(@CountFrom IS NULL AND @CountTo IS NULL) 
		OR (@CountFrom IS NULL AND t.[CurrentCount] <= @CountTo)
		OR (@CountTo IS NULL AND t.[CurrentCount] >= @CountFrom)
		OR (t.[CurrentCount] BETWEEN @CountFrom AND @CountTo)
	)
	AND (
		(@PriceFrom IS NULL AND @PriceTo IS NULL) 
		OR (@PriceFrom IS NULL AND t.Price <= @PriceTo)
		OR (@PriceTo IS NULL AND t.Price >= @PriceFrom)
		OR (t.Price BETWEEN @PriceFrom AND @PriceTo)
	)
	AND (
		(@TotalPriceFrom IS NULL AND @TotalPriceTo IS NULL) 
		OR (@TotalPriceFrom IS NULL AND Price * [CurrentCount] <= @TotalPriceTo)
		OR (@TotalPriceTo IS NULL AND Price * [CurrentCount] >= @TotalPriceFrom)
		OR (Price * [CurrentCount] BETWEEN @TotalPriceFrom AND @TotalPriceTo)
	)
	ORDER BY
	CASE WHEN @SortColumn = 'TextileId' AND @SortType ='ASC' THEN TextileName END ASC,
	CASE WHEN @SortColumn = 'TextileId' AND @SortType ='DESC' THEN TextileName END DESC,
	CASE WHEN @SortColumn = 'CompoundId' AND @SortType ='ASC' THEN CompoundName END ASC,
	CASE WHEN @SortColumn = 'CompoundId' AND @SortType ='DESC' THEN CompoundName END DESC,
	CASE WHEN @SortColumn = 'Count' AND @SortType ='ASC' THEN [CurrentCount] END ASC,
	CASE WHEN @SortColumn = 'Count' AND @SortType ='DESC' THEN [CurrentCount] END DESC,
	CASE WHEN @SortColumn = 'Price' AND @SortType ='ASC' THEN Price END ASC,
	CASE WHEN @SortColumn = 'Price' AND @SortType ='DESC' THEN Price END DESC,
	CASE WHEN @SortColumn = 'TotalPrice' AND @SortType ='ASC' THEN Price * [CurrentCount] END ASC,
	CASE WHEN @SortColumn = 'TotalPrice' AND @SortType ='DESC' THEN Price * [CurrentCount] END DESC