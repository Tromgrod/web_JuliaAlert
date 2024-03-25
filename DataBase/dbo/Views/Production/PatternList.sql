CREATE VIEW [dbo].[PatternList]
AS 
	SELECT 
	p.PatternId,
	p.ConstructorId,
	p.LocationStorageId,
	p.Code,
	p.CollectionId
	FROM [Pattern] p
	WHERE p.DeletedBy IS NULL