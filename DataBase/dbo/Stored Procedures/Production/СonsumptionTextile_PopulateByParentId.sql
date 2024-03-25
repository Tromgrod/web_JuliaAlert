CREATE PROCEDURE [dbo].[СonsumptionTextile_PopulateByParentId]
	@UniqueProductId bigint
AS
	SELECT ct.*,
	tc.ColorProductId,
	t.TextileId, t.[Name] TextileName
	FROM СonsumptionTextile ct
	JOIN TextileColor tc ON tc.TextileColorId = ct.TextileColorId
	JOIN Textile t ON t.TextileId = tc.TextileId
	WHERE ct.DeletedBy IS NULL AND ct.UniqueProductId = @UniqueProductId