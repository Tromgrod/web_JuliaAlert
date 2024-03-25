CREATE PROCEDURE [dbo].[UniqueProduct_PopulateAll]
AS
	SELECT up.UniqueProductId, up.ProductId, up.ColorProductId, up.DecorId,
	cp.[Name] ColorProductName, d.[Name] DecorName, p.[Name] ProductName,
	cp.Code ColorProductCode, d.Code DecorCode, p.Code ProductCode
	FROM UniqueProduct up
	JOIN Product p ON p.ProductId = up.ProductId
	JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId
	JOIN Decor d ON d.DecorId = up.DecorId
	WHERE up.DeletedBy IS NULL 
	AND up.[Enabled] = 1