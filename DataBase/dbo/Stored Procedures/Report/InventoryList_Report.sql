CREATE PROCEDURE [dbo].[InventoryList_Report]
	@SortColumn NVarChar(100),
	@SortType NVarChar(4),
	@InventoryId BigInt,
	@StockId BigInt,
	@IsInventoryItem bit,
	@ProductWithCurrentCount bit,
	@UniqueProductId BigInt = NULL,
	@ProductId BigInt = NULL,
	@ProductSizeId BigInt = NULL,
	@GroupProductId BigInt = NULL,
	@TypeProductId BigInt = NULL,
	@CompoundId BigInt = NULL,
	@ColorProductId BigInt = NULL,
	@DecorId BigInt = NULL,
	@CountInStockFrom Int = NULL,
	@CountInStockTo Int = NULL,
	@CurrentCountFrom Int = NULL,
	@CurrentCountTo Int = NULL,
	@DifferenceFrom Int = NULL,
	@DifferenceTo Int = NULL
AS
	SELECT * FROM (
		SELECT
		sp.SpecificProductId,
		up.UniqueProductId,
		(p.Code + '-' + cp.Code + '-' + d.Code) Code,
		(p.[Name] + ' ' + cp.[Name] + ' ' + d.[Name]) ProductName,
		ps.[Name] ProductSizeName,
		tp.[Name] TypeProductName,
		c.[Name] CompoundName,
		cp.[Name] ColorProductName,
		d.[Name] DecorName,
		ISNULL(SUM(spsh.[Count]), 0) CountInStock,
		ISNULL(SUM(iu.CurrentCount), 0) CurrentCount,
		ISNULL(SUM(spsh.[Count]), 0) + ISNULL(SUM(iu.CurrentCount), 0) [Difference]
		FROM SpecificProduct sp
		JOIN UniqueProduct up ON up.UniqueProductId = sp.UniqueProductId AND up.DeletedBy IS NULL
		JOIN Product p ON p.ProductId = up.ProductId AND p.DeletedBy IS NULL
		LEFT JOIN Compound c ON c.CompoundId = up.CompoundId AND c.DeletedBy IS NULL
		JOIN TypeProduct tp ON tp.TypeProductId = p.TypeProductId
		JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId AND cp.DeletedBy IS NULL
		JOIN Decor d ON d.DecorId = up.DecorId AND d.DeletedBy IS NULL
		JOIN ProductSize ps ON ps.ProductSizeId = sp.ProductSizeId AND ps.DeletedBy IS NULL
		LEFT JOIN SpecificProductStock sps ON sps.SpecificProductId = sp.SpecificProductId AND sps.DeletedBy IS NULL
		JOIN Inventory i ON i.InventoryId = @InventoryId AND i.DeletedBy IS NULL
		LEFT JOIN InventoryUnit iu ON iu.InventoryId = i.InventoryId AND iu.SpecificProductStockId = sps.SpecificProductStockId AND iu.DeletedBy IS NULL
		LEFT JOIN SpecificProductStockHistory spsh ON spsh.SpecificProductStockId = sps.SpecificProductStockId AND spsh.DeletedBy IS NULL
		WHERE sp.DeletedBy IS NULL
		AND (
			sps.SpecificProductStockId IS NULL OR (
				sps.StockId = @StockId
				AND DATEFROMPARTS(YEAR(spsh.[Date]), MONTH(spsh.[Date]), DAY(spsh.[Date])) <= DATEFROMPARTS(YEAR(i.[Date]), MONTH(i.[Date]), DAY(i.[Date]))
			)
		)
		AND (@IsInventoryItem = 0 OR iu.InventoryUnitId IS NOT NULL)
		AND (@UniqueProductId IS NULL OR sp.UniqueProductId = @UniqueProductId)
		AND (@ProductId IS NULL OR p.ProductId = @ProductId)
		AND (@ProductSizeId IS NULL OR sp.ProductSizeId = @ProductSizeId)
		AND (@GroupProductId IS NULL OR tp.GroupProductId = @GroupProductId)
		AND (@TypeProductId IS NULL OR tp.TypeProductId = @TypeProductId)
		AND (@CompoundId IS NULL OR c.CompoundId = @CompoundId)
		AND (@ColorProductId IS NULL OR cp.ColorProductId = @ColorProductId)
		AND (@DecorId IS NULL OR d.DecorId = @DecorId)
		GROUP BY sp.SpecificProductId, up.UniqueProductId, p.Code, cp.Code, d.Code, p.[Name], cp.[Name], d.[Name], ps.[Name], tp.[Name], c.[Name], cp.[Name], d.[Name]
	) t
	WHERE (@ProductWithCurrentCount = 0 OR t.CountInStock != 0)
	AND (
		(@CountInStockFrom IS NULL AND @CountInStockTo IS NULL) 
		OR (@CountInStockFrom IS NULL AND t.CountInStock <= @CountInStockTo)
		OR (@CountInStockTo IS NULL AND t.CountInStock >= @CountInStockFrom)
		OR (t.CountInStock BETWEEN @CountInStockFrom AND @CountInStockTo)
	) 
	AND (
		(@CurrentCountFrom IS NULL AND @CurrentCountTo IS NULL) 
		OR (@CurrentCountFrom IS NULL AND t.CurrentCount <= @CurrentCountTo)
		OR (@CurrentCountTo IS NULL AND t.CurrentCount >= @CurrentCountFrom)
		OR (t.CurrentCount BETWEEN @CurrentCountFrom AND @CurrentCountTo)
	)
	AND (
		(@DifferenceFrom IS NULL AND @DifferenceTo IS NULL) 
		OR (@DifferenceFrom IS NULL AND t.[Difference] <= @DifferenceTo)
		OR (@DifferenceTo IS NULL AND t.[Difference] >= @DifferenceFrom)
		OR (t.[Difference] BETWEEN @DifferenceFrom AND @DifferenceTo)
	)
	ORDER BY
	CASE WHEN @SortColumn = 'Code' AND @SortType ='ASC' THEN Code END ASC,
	CASE WHEN @SortColumn = 'Code' AND @SortType ='DESC' THEN Code END DESC,
	CASE WHEN @SortColumn = 'ProductSizeId' AND @SortType ='ASC' THEN ProductSizeName END ASC,
	CASE WHEN @SortColumn = 'ProductSizeId' AND @SortType ='DESC' THEN ProductSizeName END DESC,
	CASE WHEN @SortColumn = 'TypeProductId' AND @SortType ='ASC' THEN TypeProductName END ASC,
	CASE WHEN @SortColumn = 'TypeProductId' AND @SortType ='DESC' THEN TypeProductName END DESC,
	CASE WHEN @SortColumn = 'CompoundId' AND @SortType ='ASC' THEN CompoundName END ASC,
	CASE WHEN @SortColumn = 'CompoundId' AND @SortType ='DESC' THEN CompoundName END DESC,
	CASE WHEN @SortColumn = 'ColorProductId' AND @SortType ='ASC' THEN ColorProductName END ASC,
	CASE WHEN @SortColumn = 'ColorProductId' AND @SortType ='DESC' THEN ColorProductName END DESC,
	CASE WHEN @SortColumn = 'DecorId' AND @SortType ='ASC' THEN DecorName END ASC,
	CASE WHEN @SortColumn = 'DecorId' AND @SortType ='DESC' THEN DecorName END DESC,
	CASE WHEN @SortColumn = 'CountInStock' AND @SortType ='ASC' THEN CountInStock END ASC,
	CASE WHEN @SortColumn = 'CountInStock' AND @SortType ='DESC' THEN CountInStock END DESC,
	CASE WHEN @SortColumn = 'CurrentCount' AND @SortType ='ASC' THEN CurrentCount END ASC,
	CASE WHEN @SortColumn = 'CurrentCount' AND @SortType ='DESC' THEN CurrentCount END DESC,
	CASE WHEN @SortColumn = 'Difference' AND @SortType ='ASC' THEN [Difference] END ASC,
	CASE WHEN @SortColumn = 'Difference' AND @SortType ='DESC' THEN [Difference] END DESC