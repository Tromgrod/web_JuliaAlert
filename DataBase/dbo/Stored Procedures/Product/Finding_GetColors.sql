CREATE PROCEDURE [dbo].[Finding_GetColors]
	@FindingId bigint
AS
	SELECT cp.ColorProductId, cp.[Name], cp.Code
	FROM Finding f 
	JOIN FindingColor fc ON fc.FindingId = f.FindingId
	JOIN ColorProduct cp ON cp.ColorProductId = fc.ColorProductId AND cp.DeletedBy IS NULL 
	WHERE f.DeletedBy IS NULL
	AND f.FindingId = @FindingId