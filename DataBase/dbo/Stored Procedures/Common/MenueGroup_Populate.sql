CREATE PROCEDURE [dbo].[MenueGroup_Populate]
AS
	SELECT mg.MenuGroupId, mg.Name, mg.[Permission],mg.Visible,
	mt.MenuTypeId,mt.Alias as MenuTypeAlias,mt.Controller as MenuTypeController,mt.Name as MenuTypeName
	FROM MenuGroup mg
	INNER JOIN MenuType mt ON mt.MenuTypeId=mg.MenuTypeId
	WHERE mg.DeletedBy IS NULL
	ORDER BY mg.SortOrder
		 
	SELECT mi.MenuItemId, mi.Name, mi.[Permission],mi.MenuGroupId,mi.[Object],mi.[Namespace],mi.Visible
	,p.PageId,p.PageObjectId,p.Permission as PagePermission,
	mt.MenuTypeId,mt.Alias as MenuTypeAlias,mt.Controller as MenuTypeController,mt.Name as MenuTypeName
	,mtp.MenuTypeId as PageMenuTypeId,mtp.Controller as PageMenuTypeController
	FROM MenuItem mi
	INNER JOIN MenuGroup mg ON mg.MenuGroupId=mi.MenuGroupId
	INNER JOIN MenuType mt ON mt.MenuTypeId=mi.MenuTypeId
	LEFT JOIN [Page] p ON p.PageId=mi.PageId
	LEFT JOIN  MenuType mtp ON mtp.MenuTypeId=p.MenuTypeId
	WHERE mi.DeletedBy IS NULL AND mg.DeletedBy IS NULL
	ORDER BY mi.SortOrder