CREATE PROCEDURE [dbo].[SupplySpecificProductUnitList_Report]
	@SupplySpecificProductId BigInt,
	@UniqueProductId BigInt = NULL,
	@ProductId BigInt = NULL,
	@ProductSizeId BigInt = NULL,
	@FactoryTailoringId BigInt = NULL,
	@FactoryCutId BigInt = NULL,
	@ScheduledCountFrom Int = NULL,
	@ScheduledCountTo Int = NULL,
	@SupplyCountFrom Int = NULL,
	@SupplyCountTo Int = NULL
AS
	SELECT * FROM (
		SELECT
		sspu.SupplySpecificProductId,
		p.[Name] + ' ' + cp.[Name] + ' ' + d.[Name] ProductName,
		p.Code + '-' + cp.Code + '-' + d.Code Code,
		fc.[Name] FactoryCutName,
		sspu.CutCost,
		sspu.[Count] ScheduledCount,
		ps.[Name] ProductSizeName,
		SUM(ISNULL(isspu.[Count], 0)) SupplyCount
		FROM SupplySpecificProductUnit sspu
		LEFT JOIN ImplementSupplySpecificProductUnit isspu ON isspu.SupplySpecificProductUnitId = sspu.SupplySpecificProductUnitId AND isspu.DeletedBy IS NULL
		JOIN Factory fc ON fc.FactoryId = sspu.FactoryCutId AND fc.DeletedBy IS NULL
		JOIN SpecificProduct sp ON sp.SpecificProductId = sspu.SpecificProductId AND sp.DeletedBy IS NULL
		JOIN UniqueProduct up ON up.UniqueProductId = sp.UniqueProductId AND up.DeletedBy IS NULL
		JOIN Product p ON p.ProductId = up.ProductId AND p.DeletedBy IS NULL
		JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId AND cp.DeletedBy IS NULL
		JOIN Decor d ON d.DecorId = up.DecorId AND d.DeletedBy IS NULL
		JOIN ProductSize ps ON ps.ProductSizeId = sp.ProductSizeId AND ps.DeletedBy IS NULL
		WHERE sspu.DeletedBy IS NULL
		AND sspu.SupplySpecificProductId = @SupplySpecificProductId
		AND (@UniqueProductId IS NULL OR up.UniqueProductId = @UniqueProductId)
		AND (@ProductId IS NULL OR p.ProductId = @ProductId)
		AND (@FactoryCutId IS NULL OR sspu.FactoryCutId = @FactoryCutId)
		AND (@ProductSizeId IS NULL OR sp.ProductSizeId = @ProductSizeId)
		GROUP BY sspu.SupplySpecificProductId, p.[Name], cp.[Name], d.[Name], p.Code, cp.Code, d.Code, fc.[Name], sspu.CutCost, sspu.[Count], ps.[Name]
	) t
	WHERE (
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