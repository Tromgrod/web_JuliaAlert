CREATE PROCEDURE [dbo].[FindingLocationStorage_PopulateByFindingColorAndLocationStorage]
	@LocationStorageId bigint,
	@FindingColorId bigint
AS
	SELECT *
	FROM FindingLocationStorage
	WHERE LocationStorageId = @LocationStorageId
	AND FindingColorId = @FindingColorId
	AND DeletedBy IS NULL