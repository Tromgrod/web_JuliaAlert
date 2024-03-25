CREATE PROCEDURE [dbo].[ReturnSupplyTextileUnit_PopulateById]
	@ReturnSupplyTextileUnitId bigint
AS
	SELECT rstu.*,
	stu.TextileColorId
	FROM ReturnSupplyTextileUnit rstu
	JOIN SupplyTextileUnit stu ON stu.SupplyTextileUnitId = rstu.SupplyTextileUnitId
	WHERE rstu.DeletedBy IS NULL
	AND rstu.ReturnSupplyTextileUnitId = @ReturnSupplyTextileUnitId