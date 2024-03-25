CREATE PROCEDURE [dbo].[InventoryUnit_PopulateBySpecificProductStockIdAndInventoryId]
	@SpecificProductStockId bigint,
	@InventoryId bigint
AS
	SELECT *
	FROM InventoryUnit iu
	WHERE iu.DeletedBy IS NULL
	AND iu.InventoryId = @InventoryId
	AND iu.SpecificProductStockId = @SpecificProductStockId