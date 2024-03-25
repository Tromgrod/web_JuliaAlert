CREATE PROCEDURE [dbo].[MovingProductList_Report]
	@SortColumn NVarChar(100),
	@SortType NVarChar(4),
	@MovingProductId BigInt = NULL,
	@ProductId BigInt = NULL,
	@UniqueProductId BigInt = NULL,
	@ProductSizeId BigInt = NULL,
	@StockFromId BigInt = NULL,
	@StockToId BigInt = NULL,
	@DateFrom DateTime = NULL,
	@DateTo DateTime = NULL,
	@MovingCountFrom Int = NULL,
	@MovingCountTo Int = NULL
AS
	SELECT t.* FROM (
		SELECT 
		mp.MovingProductId, mp.DocumentNumber,
		mp.[Date],
		mp.[Description],
		SUM(mpu.[Count]) MovingCount
		FROM MovingProduct mp
		JOIN MovingProductUnit mpu ON mpu.MovingProductId = mp.MovingProductId AND mpu.DeletedBy IS NULL
		JOIN SpecificProduct sp ON sp.SpecificProductId = mpu.SpecificProductId AND sp.DeletedBy IS NULL
		JOIN UniqueProduct up ON up.UniqueProductId = sp.UniqueProductId AND up.DeletedBy IS NULL
		WHERE mp.DeletedBy IS NULL
		AND (@MovingProductId IS NULL OR mp.MovingProductId = @MovingProductId)
		AND (@ProductId IS NULL OR up.ProductId = @ProductId)
		AND (@UniqueProductId IS NULL OR up.UniqueProductId = @UniqueProductId)
		AND (@ProductSizeId IS NULL OR sp.ProductSizeId = @ProductSizeId)
		AND (@StockFromId IS NULL OR mpu.StockFromId = @StockFromId)
		AND (@StockToId IS NULL OR mpu.StockToId = @StockToId)
		AND (
			(@DateFrom IS NULL AND @DateTo IS NULL) 
			OR (@DateFrom IS NULL AND mp.[Date] <= @DateTo)
			OR (@DateTo IS NULL AND mp.[Date] >= @DateFrom)
			OR (mp.[Date] BETWEEN @DateFrom AND @DateTo)
		)
		GROUP BY mp.MovingProductId, mp.DocumentNumber, mp.[Date], mp.[Description]
	) t
	WHERE (
		(@MovingCountFrom IS NULL AND @MovingCountTo IS NULL) 
		OR (@MovingCountFrom IS NULL AND t.MovingCount <= @MovingCountTo)
		OR (@MovingCountTo IS NULL AND t.MovingCount >= @MovingCountFrom)
		OR (t.MovingCount BETWEEN @MovingCountFrom AND @MovingCountTo)
	)
	ORDER BY 
	CASE WHEN @SortColumn = 'MovingProduct' AND @SortType ='ASC' THEN MovingProductId END ASC,
	CASE WHEN @SortColumn = 'MovingProduct' AND @SortType ='DESC' THEN MovingProductId END DESC,
	CASE WHEN @SortColumn = 'Date' AND @SortType ='ASC' THEN [Date] END ASC,
	CASE WHEN @SortColumn = 'Date' AND @SortType ='DESC' THEN [Date] END DESC,
	CASE WHEN @SortColumn = 'MovingCount' AND @SortType ='ASC' THEN MovingCount END ASC,
	CASE WHEN @SortColumn = 'MovingCount' AND @SortType ='DESC' THEN MovingCount END DESC