CREATE PROCEDURE [dbo].[Populate_UniqueProduct_ByProduct]
	@ProductId bigint
AS
	SELECT up.*,
	p.[Name] AS ProductName, cp.[Name] AS ColorProductName, d.[Name] AS DecorName,
	cp.Code AS ColorProductCode, d.Code AS DecorCode, p.Code AS ProductCode
	FROM UniqueProduct up
	JOIN Product p ON p.ProductId = up.ProductId
	JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId
	JOIN Decor d ON d.DecorId = up.DecorId
	WHERE up.ProductId = @ProductId 
	AND up.DeletedBy IS NULL