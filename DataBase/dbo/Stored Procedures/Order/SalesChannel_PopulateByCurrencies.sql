CREATE PROCEDURE [dbo].[SalesChannel_PopulateByCurrencies]
	@Currencies NvarChar(100)
AS
	SELECT * 
	FROM SalesChannel 
	WHERE DeletedBy IS NULL 
	AND CurrencyId IN (SELECT TRY_CONVERT(BIGINT, RTRIM(value)) FROM STRING_SPLIT(@Currencies, ',') WHERE RTRIM(value) != '')