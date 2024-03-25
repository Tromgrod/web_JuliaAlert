CREATE PROCEDURE [dbo].[Stock_PopulateByCurrencies]
	@Currencies NvarChar(100)
AS
	SELECT * 
	FROM Stock 
	WHERE DeletedBy IS NULL
	AND (IsMainStock = 1 OR CurrencyId IN (SELECT TRY_CONVERT(BIGINT, RTRIM(value)) FROM STRING_SPLIT(@Currencies, ',') WHERE RTRIM(value) != ''))