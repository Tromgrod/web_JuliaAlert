CREATE PROCEDURE [dbo].[FindingList_Report]
	@SortColumn NVarChar(100),
	@SortType NVarChar(4),
	@FindingId BigInt = NULL,
	@FindingSpecieId BigInt = NULL,
	@FindingSubspecieId BigInt = NULL,
	@LocationStorageId BigInt = NULL,
	@CountFrom Decimal(10, 2) = NULL,
	@CountTo Decimal(10, 2) = NULL,
	@PriceFrom Decimal(10, 2) = NULL,
	@PriceTo Decimal(10, 2) = NULL,
	@TotalPriceFrom Decimal(10, 2) = NULL,
	@TotalPriceTo Decimal(10, 2) = NULL
AS
	SELECT t.*, Price * [Count] TotalPrice FROM ( 
		SELECT
		f.FindingId,
		fs.[Name] FindingSpecieName,
		fsubs.[Name] FindingSubspecieName,
		STRING_AGG(cl.[Name], ', ') Colors,
		ISNULL(last_sf.Price, 0) Price,
		SUM(ISNULL(fls.CurrentCount, 0)) [Count]
		FROM Finding f
		JOIN FindingColor fc ON fc.FindingId = f.FindingId
		JOIN ColorProduct cl ON cl.ColorProductId = fc.ColorProductId
		JOIN FindingSubspecie fsubs ON fsubs.FindingSubspecieId = f.FindingSubspecieId
		JOIN FindingSpecie fs ON fs.FindingSpecieId = fsubs.FindingSpecieId
		LEFT JOIN FindingLocationStorage fls ON fls.FindingColorId = fc.FindingColorId
		LEFT JOIN (
			SELECT TOP 1 WITH TIES f.FindingId, sfu.Price
			FROM Finding f
			JOIN FindingColor fc ON fc.FindingId = f.FindingId
			LEFT JOIN SupplyFindingUnit sfu ON sfu.FindingColorId = fc.FindingColorId
			LEFT JOIN SupplyFinding sf ON sf.SupplyFindingId = sfu.SupplyFindingId
			ORDER BY ROW_NUMBER() OVER(PARTITION BY f.FindingId ORDER BY sf.[Date] DESC)
		) last_sf ON last_sf.FindingId = f.FindingId
		WHERE f.DeletedBy IS NULL
		AND (@FindingId IS NULL OR f.FindingId = @FindingId)
		AND (@FindingSpecieId IS NULL OR fs.FindingSpecieId = @FindingSpecieId)
		AND (@FindingSubspecieId IS NULL OR fsubs.FindingSubspecieId = @FindingSubspecieId)
		AND (@LocationStorageId IS NULL OR fls.LocationStorageId = @LocationStorageId)
		GROUP BY f.FindingId, fs.[Name], fsubs.[Name], last_sf.Price
	) t
	WHERE (
		(@CountFrom IS NULL AND @CountTo IS NULL) 
		OR (@CountFrom IS NULL AND t.[Count] <= @CountTo)
		OR (@CountTo IS NULL AND t.[Count] >= @CountFrom)
		OR (t.[Count] BETWEEN @CountFrom AND @CountTo)
	)
	AND (
		(@PriceFrom IS NULL AND @PriceTo IS NULL) 
		OR (@PriceFrom IS NULL AND t.Price <= @PriceTo)
		OR (@PriceTo IS NULL AND t.Price >= @PriceFrom)
		OR (t.Price BETWEEN @PriceFrom AND @PriceTo)
	)
	AND (
		(@TotalPriceFrom IS NULL AND @TotalPriceTo IS NULL) 
		OR (@TotalPriceFrom IS NULL AND Price * [Count] <= @TotalPriceTo)
		OR (@TotalPriceTo IS NULL AND Price * [Count] >= @TotalPriceFrom)
		OR (Price * [Count] BETWEEN @TotalPriceFrom AND @TotalPriceTo)
	)
	ORDER BY
	CASE WHEN @SortColumn = 'Finding' AND @SortType ='ASC' THEN FindingSpecieName END ASC,
	CASE WHEN @SortColumn = 'Finding' AND @SortType ='DESC' THEN FindingSpecieName END DESC,
	CASE WHEN @SortColumn = 'Count' AND @SortType ='ASC' THEN [Count] END ASC,
	CASE WHEN @SortColumn = 'Count' AND @SortType ='DESC' THEN [Count] END DESC,
	CASE WHEN @SortColumn = 'Price' AND @SortType ='ASC' THEN Price END ASC,
	CASE WHEN @SortColumn = 'Price' AND @SortType ='DESC' THEN Price END DESC,
	CASE WHEN @SortColumn = 'TotalPrice' AND @SortType ='ASC' THEN Price * [Count] END ASC,
	CASE WHEN @SortColumn = 'TotalPrice' AND @SortType ='DESC' THEN Price * [Count] END DESC