CREATE PROCEDURE [dbo].[Field_Populate_Page]
	@PageObjectId nvarchar(50),
	@LaboratoryId bigint = null
AS
	SELECT f.FieldId,f.FieldName,f.[Name],f.PrintName,f.LaboratoryId,f.Permission
	FROM Field f
	JOIN [Page] p ON p.PageId=f.PageId
	WHERE f.LaboratoryId = @LaboratoryId AND p.PageObjectId = @PageObjectId
	ORDER BY f.DateCreated ASC