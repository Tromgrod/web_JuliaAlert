CREATE PROCEDURE [dbo].[SupplySpecificProductList_Report]
	@SortColumn NVarChar(100),
	@SortType NVarChar(4),
	@SupplySpecificProductId BigInt = NULL,
	@DateFrom DateTime = NULL,
	@DateTo DateTime = NULL,
	@UniqueProductId BigInt = NULL,
	@ProductId BigInt = NULL,
	@ProductSizeId BigInt = NULL,
	@FactoryTailoringId BigInt = NULL,
	@TailoringCostAvgFrom Decimal(10, 2) = NULL,
	@TailoringCostAvgTo Decimal(10, 2) = NULL,
	@FactoryCutId BigInt = NULL,
	@CutCostAvgFrom Decimal(10, 2) = NULL,
	@CutCostAvgTo Decimal(10, 2) = NULL,
	@ScheduledCountFrom Int = NULL,
	@ScheduledCountTo Int = NULL,
	@SupplyCountFrom Int = NULL,
	@SupplyCountTo Int = NULL
AS
	SELECT * FROM (
		SELECT
		ssp.SupplySpecificProductId,
		ssp.DocumentNumber,
		ssp.[Date],
		AVG(sspu.CutCost) CutCostAvg,
		SUM(sspu.[Count]) ScheduledCount,
		ISNULL(SUM(isspu.[Count]), 0) SupplyCount
		FROM SupplySpecificProduct ssp
		JOIN SupplySpecificProductUnit sspu ON sspu.SupplySpecificProductId = ssp.SupplySpecificProductId AND sspu.DeletedBy IS NULL
		JOIN SpecificProduct sp ON sp.SpecificProductId = sspu.SpecificProductId AND sp.DeletedBy IS NULL
		JOIN UniqueProduct up ON up.UniqueProductId = sp.UniqueProductId AND up.DeletedBy IS NULL
		JOIN Product p ON p.ProductId = up.ProductId AND p.DeletedBy IS NULL
		JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId AND cp.DeletedBy IS NULL
		JOIN Decor d ON d.DecorId = up.DecorId AND d.DeletedBy IS NULL
		CROSS APPLY (
			SELECT SUM(isspu.[Count]) AS [Count] 
			FROM ImplementSupplySpecificProductUnit isspu
			WHERE isspu.SupplySpecificProductUnitId = sspu.SupplySpecificProductUnitId AND isspu.DeletedBy IS NULL
		) isspu
		WHERE ssp.DeletedBy IS NULL
		AND (
			(@DateFrom IS NULL AND @DateTo IS NULL) 
			OR (@DateFrom IS NULL AND ssp.[Date] <= @DateTo)
			OR (@DateTo IS NULL AND ssp.[Date] >= @DateFrom)
			OR (ssp.[Date] BETWEEN @DateFrom AND @DateTo)
		)
		AND (@ProductSizeId IS NULL OR sp.ProductSizeId = @ProductSizeId)
		AND (@SupplySpecificProductId IS NULL OR ssp.SupplySpecificProductId = @SupplySpecificProductId)
		AND (@UniqueProductId IS NULL OR up.UniqueProductId = @UniqueProductId)
		AND (@ProductId IS NULL OR p.ProductId = @ProductId)
		AND (@FactoryCutId IS NULL OR sspu.FactoryCutId = @FactoryCutId)
		GROUP BY ssp.SupplySpecificProductId, ssp.DocumentNumber, ssp.[Date]
	) t
	WHERE (
		(@CutCostAvgFrom IS NULL AND @CutCostAvgTo IS NULL) 
		OR (@CutCostAvgFrom IS NULL AND t.CutCostAvg <= @CutCostAvgTo)
		OR (@CutCostAvgTo IS NULL AND t.CutCostAvg >= @CutCostAvgFrom)
		OR (t.CutCostAvg BETWEEN @CutCostAvgFrom AND @CutCostAvgTo)
	)
	AND (
		(@ScheduledCountFrom IS NULL AND @ScheduledCountTo IS NULL) 
		OR (@ScheduledCountFrom IS NULL AND t.ScheduledCount <= @ScheduledCountTo)
		OR (@ScheduledCountTo IS NULL AND t.ScheduledCount >= @ScheduledCountFrom)
		OR (t.ScheduledCount BETWEEN @ScheduledCountFrom AND @ScheduledCountTo)
	)
	AND (
		(@SupplyCountFrom IS NULL AND @SupplyCountTo IS NULL) 
		OR (@SupplyCountFrom IS NULL AND t.SupplyCount <= @SupplyCountTo)
		OR (@SupplyCountTo IS NULL AND t.SupplyCount >= @SupplyCountFrom)
		OR (t.SupplyCount BETWEEN @SupplyCountFrom AND @SupplyCountTo)
	)
	ORDER BY 
	CASE WHEN @SortColumn = 'Date' AND @SortType ='ASC' THEN [Date] END ASC,
	CASE WHEN @SortColumn = 'Date' AND @SortType ='DESC' THEN [Date] END DESC,
	CASE WHEN @SortColumn = 'CutCostAvg' AND @SortType ='ASC' THEN CutCostAvg END ASC,
	CASE WHEN @SortColumn = 'CutCostAvg' AND @SortType ='DESC' THEN CutCostAvg END DESC,
	CASE WHEN @SortColumn = 'ScheduledCount' AND @SortType ='ASC' THEN ScheduledCount END ASC,
	CASE WHEN @SortColumn = 'ScheduledCount' AND @SortType ='DESC' THEN ScheduledCount END DESC,
	CASE WHEN @SortColumn = 'SupplyCount' AND @SortType ='ASC' THEN SupplyCount END ASC,
	CASE WHEN @SortColumn = 'SupplyCount' AND @SortType ='DESC' THEN SupplyCount END DESC