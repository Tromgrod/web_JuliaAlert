CREATE PROCEDURE [dbo].[Inventory_PopulateAutocomplete]
	@search NVarChar(MAX),
	@param NVarChar(MAX) = NULL
AS
	SELECT TOP(10) i.InventoryId, i.[Date]
	FROM Inventory i
	WHERE i.DeletedBy IS NULL
	AND (@search = '' OR i.[Date] LIKE '%' + @search + '%' )
	ORDER BY i.[Date]