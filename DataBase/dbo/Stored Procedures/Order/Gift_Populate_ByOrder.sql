CREATE PROCEDURE [dbo].[Gift_Populate_ByOrder] (
	@OrderId bigint
)
AS
	SELECT g.*,
	up.*, sp.ProductSizeId,
	cp.[Name] AS ColorProductName, d.[Name] AS DecorName, p.[Name] AS ProductName, ps.[Name] AS ProductSizeName,
	cp.Code AS ColorProductCode, d.Code AS DecorCode, p.Code AS ProductCode
	FROM Gift g
	JOIN SpecificProduct sp ON sp.SpecificProductId = g.SpecificProductId
	JOIN UniqueProduct up ON up.UniqueProductId = sp.UniqueProductId
	JOIN Product p ON p.ProductId = up.ProductId
	JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId
	JOIN Decor d ON d.DecorId = up.DecorId
	JOIN ProductSize ps ON ps.ProductSizeId = sp.ProductSizeId
	WHERE g.DeletedBy IS NULL AND g.OrderId = @OrderId
	ORDER BY g.DateCreated DESC