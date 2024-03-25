CREATE PROCEDURE [dbo].[Textile_GetColors]
	@TextileId bigint
AS
	SELECT cp.ColorProductId, cp.[Name], cp.Code
	FROM Textile t
	JOIN TextileColor tc ON tc.TextileId = t.TextileId
	JOIN ColorProduct cp ON cp.ColorProductId = tc.ColorProductId AND cp.DeletedBy IS NULL 
	WHERE t.DeletedBy IS NULL
	AND t.TextileId = @TextileId