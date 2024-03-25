CREATE PROCEDURE [dbo].[SpecificProduct_GenerateProductCode]
	@Count INT = 1
AS
	SELECT t.val ProductCode, ROW_NUMBER() OVER(ORDER BY t.val ASC) AS [Row] FROM (
		SELECT TOP(@Count) numbers.val
		FROM (
			SELECT ones.n + 10 * tens.n + 100 * hundreds.n + 1000 * thousands.n + 10000 * tens_of_housands.n AS val
			FROM 
			(VALUES(0),(1),(2),(3),(4),(5),(6),(7),(8),(9)) ones(n),
			(VALUES(0),(1),(2),(3),(4),(5),(6),(7),(8),(9)) tens(n),
			(VALUES(0),(1),(2),(3),(4),(5),(6),(7),(8),(9)) hundreds(n),
			(VALUES(0),(1),(2),(3),(4),(5),(6),(7),(8),(9)) thousands(n),
			(VALUES(0),(1),(2),(3),(4),(5),(6),(7),(8),(9)) tens_of_housands(n)
		) numbers
		WHERE numbers.val NOT IN (SELECT ProductCode FROM SpecificProduct)
	) t