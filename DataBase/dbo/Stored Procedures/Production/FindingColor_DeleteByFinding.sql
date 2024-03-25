CREATE PROCEDURE [dbo].[FindingColor_DeleteByFinding]
	@FindingId bigint,
	@ColorId bigint
AS
	DELETE FindingColor
	WHERE FindingId = @FindingId AND ColorProductId = @ColorId