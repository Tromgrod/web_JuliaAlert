CREATE PROCEDURE [dbo].[GiftList_Report]
	@SortColumn NVarChar(100),
	@SortType NVarChar(4),
	@UniqueProductId BigInt = NULL,
	@ProductId BigInt = NULL,
	@ClientId BigInt = NULL,
	@CountFrom Int = NULL,
	@CountTo Int = NULL,
	@GiftDateFrom DateTime = NULL,
	@GiftDateTo DateTime = NULL
AS
	SELECT
	o.OrderId,
	p.[Name] + ' ' + cp.[Name] + ' ' + d.[Name] ProductName,
	p.Code + '-' + cp.Code + '-' + d.Code Code,
	g.[Count],
	cl.[Name] ClientName, 
	o.OrderDate GiftDate
	FROM Gift g
	JOIN SpecificProduct sp ON sp.SpecificProductId = g.SpecificProductId AND sp.DeletedBy IS NULL
	JOIN UniqueProduct up ON up.UniqueProductId = sp.UniqueProductId AND up.DeletedBy IS NULL
	JOIN Product p ON p.ProductId = up.ProductId AND p.DeletedBy IS NULL
	JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId AND cp.DeletedBy IS NULL
	JOIN Decor d ON d.DecorId = up.DecorId AND d.DeletedBy IS NULL
	JOIN [Order] o ON o.OrderId = g.OrderId AND o.DeletedBy IS NULL
	JOIN Client cl ON cl.ClientId = o.ClientId AND cl.DeletedBy IS NULL
	WHERE g.DeletedBy IS NULL
	AND up.DeletedBy IS NULL
	AND (@UniqueProductId IS NULL OR up.UniqueProductId = @UniqueProductId)
	AND (@ProductId IS NULL OR p.ProductId = @ProductId)
	AND (@ClientId IS NULL OR cl.ClientId = @ClientId)
	AND (
		(@CountFrom IS NULL AND @CountTo IS NULL) 
		OR (@CountFrom IS NULL AND g.[Count] <= @CountTo)
		OR (@CountTo IS NULL AND g.[Count] >= @CountFrom)
		OR (g.[Count] BETWEEN @CountFrom AND @CountTo)
	)
	AND (
		(@GiftDateFrom IS NULL AND @GiftDateTo IS NULL) 
		OR (@GiftDateFrom IS NULL AND o.OrderDate <= @GiftDateTo)
		OR (@GiftDateTo IS NULL AND o.OrderDate >= @GiftDateFrom)
		OR (o.OrderDate BETWEEN @GiftDateFrom AND @GiftDateTo)
	)
	ORDER BY 
	CASE WHEN @SortColumn = 'Count' AND @SortType ='ASC' THEN [Count] END ASC,
	CASE WHEN @SortColumn = 'Count' AND @SortType ='DESC' THEN [Count] END DESC,
	CASE WHEN @SortColumn = 'ClientId' AND @SortType ='ASC' THEN cl.[Name] END ASC,
	CASE WHEN @SortColumn = 'ClientId' AND @SortType ='DESC' THEN cl.[Name] END DESC,
	CASE WHEN @SortColumn = 'GiftDate' AND @SortType ='ASC' THEN OrderDate END ASC,
	CASE WHEN @SortColumn = 'GiftDate' AND @SortType ='DESC' THEN OrderDate END DESC