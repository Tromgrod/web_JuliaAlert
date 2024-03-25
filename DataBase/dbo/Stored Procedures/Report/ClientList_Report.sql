CREATE PROCEDURE [dbo].[ClientList_Report]
	@CurrencyIds NVarChar(100) = NULL,
	@SortColumn NVarChar(100),
	@SortType NVarChar(4),
	@ClientId BigInt = NULL,
	@CountriesId BigInt = NULL,
	@CityId BigInt = NULL,
	@Phone NVarChar(20) = NULL,
	@DiscountFrom Int = NULL,
	@DiscountTo Int = NULL,
	@BirthdayFrom DateTime = NULL,
	@BirthdayTo DateTime = NULL,
	@TotalSUMFrom Decimal(10, 2) = NULL,
	@TotalSUMTo Decimal(10, 2) = NULL
AS
	SELECT * FROM (
		SELECT c.ClientId, c.[Name], ISNULL(c.Phone, '') Phone, ISNULL(c.[Comment], '') [Comment], ISNULL(c.Discount, 0) Discount, ISNULL(c.Birthday, '') Birthday, c.Email,
		ci.[Name] CityName, coun.[Name] CountriesName,
		SUM(pfo.[Count] * pfo.FinalPrice) - SUM(ISNULL(pfo.FinalPrice * r.ReturnCount, 0)) AS TotalSUM
		FROM Client c
		LEFT JOIN City ci ON ci.CityId = c.CityId
		LEFT JOIN Countries coun ON coun.CountriesId = c.CountriesId
		LEFT JOIN [Order] o ON o.ClientId = c.ClientId AND o.DeletedBy IS NULL
		LEFT JOIN ProductForOrder pfo ON pfo.OrderId = o.OrderId AND pfo.DeletedBy IS NULL
		LEFT JOIN SalesChannel sc ON sc.SalesChannelId = o.SalesChannelId AND sc.DeletedBy IS NULL
		CROSS APPLY (
			SELECT SUM(r.ReturnCount) ReturnCount 
			FROM [Return] r 
			WHERE r.ProductForOrderId = pfo.ProductForOrderId AND r.DeletedBy IS NULL
		) r
		WHERE c.DeletedBy IS NULL
		AND (@CurrencyIds IS NULL OR sc.CurrencyId IN (SELECT TRY_CONVERT(BIGINT, RTRIM(value)) FROM STRING_SPLIT(@CurrencyIds, ',') WHERE RTRIM(value) != ''))
		AND (@ClientId IS NULL OR c.ClientId = @ClientId)
		AND (@Phone IS NULL OR c.Phone = @Phone)
		AND (@CountriesId IS NULL OR c.CountriesId = @CountriesId)
		AND (@CityId IS NULL OR c.CityId = @CityId)
		AND (
			(@DiscountFrom IS NULL AND @DiscountTo IS NULL) 
			OR (@DiscountFrom IS NULL AND c.Discount <= @DiscountTo)
			OR (@DiscountTo IS NULL AND c.Discount >= @DiscountFrom)
			OR (c.Discount BETWEEN @DiscountFrom AND @DiscountTo)
		)
		AND (
			(@BirthdayFrom IS NULL AND @BirthdayTo IS NULL) 
			OR (@BirthdayFrom IS NULL AND DATEFROMPARTS(9999, MONTH(c.Birthday), DAY(c.Birthday)) <= DATEFROMPARTS(9999, MONTH(@BirthdayTo), DAY(@BirthdayTo)))
			OR (@BirthdayTo IS NULL AND DATEFROMPARTS(9999, MONTH(c.Birthday), DAY(c.Birthday)) >= DATEFROMPARTS(9999, MONTH(@BirthdayFrom), DAY(@BirthdayFrom)))
			OR (DATEFROMPARTS(9999, MONTH(c.Birthday), DAY(c.Birthday)) BETWEEN DATEFROMPARTS(9999, MONTH(@BirthdayFrom), DAY(@BirthdayFrom)) AND DATEFROMPARTS(9999, MONTH(@BirthdayTo), DAY(@BirthdayTo)))
		)
		GROUP BY c.ClientId, c.[Name], c.Phone, c.[Comment], c.Discount, c.Birthday, c.Email, coun.[Name], ci.[Name]
	) t
	WHERE (
		(@TotalSUMFrom IS NULL AND @TotalSUMTo IS NULL) 
		OR (@TotalSUMFrom IS NULL AND TotalSUM <= @TotalSUMTo)
		OR (@TotalSUMTo IS NULL AND TotalSUM >= @TotalSUMFrom)
		OR (TotalSUM BETWEEN @TotalSUMFrom AND @TotalSUMTo)
	)
	ORDER BY
	CASE WHEN @SortColumn = 'ClientId' AND @SortType ='ASC' THEN [Name] END ASC,
	CASE WHEN @SortColumn = 'ClientId' AND @SortType ='DESC' THEN [Name] END DESC,
	CASE WHEN @SortColumn = 'Phone' AND @SortType ='ASC' THEN Phone END ASC,
	CASE WHEN @SortColumn = 'Phone' AND @SortType ='DESC' THEN Phone END DESC,
	CASE WHEN @SortColumn = 'Discount' AND @SortType ='ASC' THEN Discount END ASC,
	CASE WHEN @SortColumn = 'Discount' AND @SortType ='DESC' THEN Discount END DESC,
	CASE WHEN @SortColumn = 'Birthday' AND @SortType ='ASC' THEN Birthday END ASC,
	CASE WHEN @SortColumn = 'Birthday' AND @SortType ='DESC' THEN Birthday END DESC,
	CASE WHEN @SortColumn = 'TotalSUM' AND @SortType ='ASC' THEN TotalSUM END ASC,
	CASE WHEN @SortColumn = 'TotalSUM' AND @SortType ='DESC' THEN TotalSUM END DESC,
	CASE WHEN @SortColumn = 'Comment' AND @SortType ='ASC' THEN Comment END ASC,
	CASE WHEN @SortColumn = 'Comment' AND @SortType ='DESC' THEN Comment END DESC