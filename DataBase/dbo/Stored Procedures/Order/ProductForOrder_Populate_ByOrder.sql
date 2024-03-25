CREATE PROCEDURE [dbo].[ProductForOrder_Populate_ByOrder] (
	@OrderId bigint
)
AS
	SELECT pfo.*,
	up.*, sp.ProductSizeId,
	cp.[Name] AS ColorProductName, d.[Name] AS DecorName, p.[Name] AS ProductName, ps.[Name] AS ProductSizeName,
	cp.Code AS ColorProductCode, d.Code AS DecorCode, p.Code AS ProductCode,
	c.[Name] AS CompoundName
	FROM ProductForOrder pfo
	JOIN SpecificProduct sp ON sp.SpecificProductId = pfo.SpecificProductId
	JOIN UniqueProduct up ON up.UniqueProductId = sp.UniqueProductId
	JOIN Product p ON p.ProductId = up.ProductId
	JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId
	JOIN Decor d ON d.DecorId = up.DecorId
	JOIN ProductSize ps ON ps.ProductSizeId = sp.ProductSizeId
	LEFT JOIN Compound c ON c.CompoundId = up.CompoundId
	WHERE pfo.DeletedBy IS NULL AND pfo.OrderId = @OrderId
	ORDER BY pfo.DateCreated DESC