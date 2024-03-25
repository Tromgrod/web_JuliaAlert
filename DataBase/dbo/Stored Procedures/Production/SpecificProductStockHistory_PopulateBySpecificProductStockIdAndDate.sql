CREATE PROCEDURE [dbo].[SpecificProductStockHistory_PopulateBySpecificProductStockIdAndDate]
	@SpecificProductStockId BigInt,
	@Date DateTime
AS
	SELECT
	spsh.SpecificProductStockId,
	SUM(spsh.[Count]) CurrentCount
	FROM SpecificProductStockHistory spsh
	WHERE DeletedBy IS NULL
	AND SpecificProductStockId = @SpecificProductStockId
	AND DATEFROMPARTS(YEAR(spsh.[Date]), MONTH(spsh.[Date]), DAY(spsh.[Date])) <= DATEFROMPARTS(YEAR(@Date), MONTH(@Date), DAY(@Date))
	GROUP BY spsh.SpecificProductStockId