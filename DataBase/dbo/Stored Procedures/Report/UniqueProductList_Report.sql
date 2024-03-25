CREATE PROCEDURE [dbo].[UniqueProductList_Report]
	@SortColumn NVarChar(100),
	@SortType NVarChar(4),
	@StockIds NVarChar(100),
	@ProductWithCurrentCount bit,
	@UniqueProductId BigInt = NULL,
	@ProductId BigInt = NULL,
	@GroupProductId BigInt = NULL,
	@TypeProductId BigInt = NULL,
	@CompoundId BigInt = NULL,
	@ColorProductId BigInt = NULL,
	@DecorId BigInt = NULL,
	@ProductSizeId BigInt = NULL,
	@StockId BigInt = NULL,
	@DateStock DateTime = NULL,
	@CurrentCountFrom Int = NULL,
	@CurrentCountTo Int = NULL
AS
	SELECT * FROM (
		SELECT 
		t.UniqueProductId,
		t.ProductName,
		t.Code,
		t.TypeProductName,
		t.CompoundName,
		t.ColorProductName,
		t.DecorName,
		t.ImageBOName, t.ImageName, t.ImageExt,
		SUM(t.CurrentCount) CurrentCount
		FROM (
			SELECT
			up.UniqueProductId,
			p.[Name] + ' ' + cp.[Name] + ' ' + d.[Name] ProductName,
			p.Code + '-' + cp.Code + '-' + d.Code Code,
			tp.[Name] TypeProductName,
			c.[Name] CompoundName,
			cp.[Name] ColorProductName,
			d.[Name] DecorName,
			g.BOName ImageBOName, g.[Name] ImageName, g.Ext ImageExt,
			ISNULL(SUM(spsh.[Count]), 0) CurrentCount
			FROM UniqueProduct up
			JOIN Product p ON p.ProductId = up.ProductId AND p.DeletedBy IS NULL
			LEFT JOIN Compound c ON c.CompoundId = up.CompoundId AND c.DeletedBy IS NULL
			LEFT JOIN TypeProduct tp ON tp.TypeProductId = p.TypeProductId
			JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId AND cp.DeletedBy IS NULL
			JOIN Decor d ON d.DecorId = up.DecorId AND d.DeletedBy IS NULL
			LEFT JOIN Graphic g ON g.GraphicId = up.ImageId AND g.DeletedBy IS NULL
			JOIN SpecificProduct sp ON sp.UniqueProductId = up.UniqueProductId AND up.DeletedBy IS NULL
			LEFT JOIN SpecificProductStock sps ON sps.SpecificProductId = sp.SpecificProductId AND sps.DeletedBy IS NULL
			LEFT JOIN SpecificProductStockHistory spsh ON spsh.SpecificProductStockId = sps.SpecificProductStockId AND spsh.DeletedBy IS NULL
			WHERE up.[Enabled] = 1
			AND up.DeletedBy IS NULL
			AND (sps.SpecificProductStockId IS NULL OR sps.StockId IN (SELECT TRY_CONVERT(BIGINT, RTRIM(value)) FROM STRING_SPLIT(@StockIds, ',') WHERE RTRIM(value) != ''))
			AND (@UniqueProductId IS NULL OR up.UniqueProductId = @UniqueProductId)
			AND (@ProductId IS NULL OR p.ProductId = @ProductId)
			AND (@GroupProductId IS NULL OR tp.GroupProductId = @GroupProductId)
			AND (@TypeProductId IS NULL OR tp.TypeProductId = @TypeProductId)
			AND (@CompoundId IS NULL OR c.CompoundId = @CompoundId)
			AND (@ColorProductId IS NULL OR cp.ColorProductId = @ColorProductId)
			AND (@DecorId IS NULL OR d.DecorId = @DecorId)
			AND (@ProductSizeId IS NULL OR sp.ProductSizeId = @ProductSizeId)
			AND (@StockId IS NULL OR sps.SpecificProductStockId IS NULL OR sps.StockId = @StockId)
			AND (@DateStock IS NULL OR sps.SpecificProductStockId IS NULL OR DATEFROMPARTS(YEAR(spsh.[Date]), MONTH(spsh.[Date]), DAY(spsh.[Date])) <= DATEFROMPARTS(YEAR(@DateStock), MONTH(@DateStock), DAY(@DateStock)))
			GROUP BY sp.SpecificProductId, up.UniqueProductId, p.[Name], cp.[Name], d.[Name], p.Code, cp.Code, d.Code, tp.[Name], c.[Name], cp.[Name], d.[Name], g.BOName, g.[Name], g.Ext
		) t
		GROUP BY t.UniqueProductId, t.ProductName, t.Code, t.TypeProductName, t.CompoundName, t.ColorProductName, t.DecorName, t.ImageBOName, t.ImageName, t.ImageExt
	) t
	WHERE (@ProductWithCurrentCount = 0 OR t.CurrentCount != 0)
	AND (
		(@CurrentCountFrom IS NULL AND @CurrentCountTo IS NULL) 
		OR (@CurrentCountFrom IS NULL AND t.CurrentCount <= @CurrentCountTo)
		OR (@CurrentCountTo IS NULL AND t.CurrentCount >= @CurrentCountFrom)
		OR (t.CurrentCount BETWEEN @CurrentCountFrom AND @CurrentCountTo)
	)
	ORDER BY 
	CASE WHEN @SortColumn = 'CurrentCount' AND @SortType ='ASC' THEN CurrentCount END ASC,
	CASE WHEN @SortColumn = 'CurrentCount' AND @SortType ='DESC' THEN CurrentCount END DESC,
	CASE WHEN @SortColumn = 'TypeProductId' AND @SortType ='ASC' THEN TypeProductName END ASC,
	CASE WHEN @SortColumn = 'TypeProductId' AND @SortType ='DESC' THEN TypeProductName END DESC,
	CASE WHEN @SortColumn = 'CompoundId' AND @SortType ='ASC' THEN CompoundName END ASC,
	CASE WHEN @SortColumn = 'CompoundId' AND @SortType ='DESC' THEN CompoundName END DESC,
	CASE WHEN @SortColumn = 'ColorProductId' AND @SortType ='ASC' THEN ColorProductName END ASC,
	CASE WHEN @SortColumn = 'ColorProductId' AND @SortType ='DESC' THEN ColorProductName END DESC,
	CASE WHEN @SortColumn = 'DecorId' AND @SortType ='ASC' THEN DecorName END ASC,
	CASE WHEN @SortColumn = 'DecorId' AND @SortType ='DESC' THEN DecorName END DESC