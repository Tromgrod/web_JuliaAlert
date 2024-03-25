CREATE PROCEDURE [dbo].[SupplyTextileUnit_PopulateById]
	@SupplyTextileUnitId bigint
AS
	SELECT stu.*,
	tc.CurrentCount TextileColorCurrentCount
	FROM SupplyTextileUnit stu
	JOIN TextileColor tc ON tc.TextileColorId = stu.TextileColorId AND tc.DeletedBy IS NULL
	WHERE stu.DeletedBy IS NULL
	AND stu.SupplyTextileUnitId = @SupplyTextileUnitId