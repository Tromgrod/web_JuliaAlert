CREATE PROCEDURE [dbo].[Populate_UniqueProduct]
	@search NVarChar(MAX) = null
AS
	SELECT TOP (15) up.UniqueProductId, up.ProductId, up.ColorProductId, up.DecorId,
	cp.[Name] ColorProductName, d.[Name] DecorName, p.[Name] ProductName,
	cp.Code ColorProductCode, d.Code DecorCode, p.Code ProductCode
	FROM UniqueProduct up
	JOIN Product p ON p.ProductId = up.ProductId
	JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId
	JOIN Decor d ON d.DecorId = up.DecorId
	WHERE up.DeletedBy IS NULL 
	AND up.[Enabled] = 1
	AND (
	@search IS NULL 
	OR p.Code + '-' + cp.Code + '-' + d.Code LIKE '%' + @search + '%' 
	OR p.Code + ' ' + cp.Code + ' ' + d.Code LIKE '%' + @search + '%'
	OR p.[Name] + ' ' + cp.[Name] + ' ' + d.[Name] LIKE '%' + @search + '%'
	OR p.[Code] + ' ' + cp.[Name] + ' ' + d.[Name] LIKE '%' + @search + '%'
	)