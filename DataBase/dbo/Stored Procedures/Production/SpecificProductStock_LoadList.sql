CREATE PROCEDURE [dbo].[SpecificProductStock_LoadList]
	@IsMainStock bit = null,
	@StockIds nvarchar(100)
AS
	SELECT TOP(20)
	sps.SpecificProductStockId, sps.CurrentCount, sps.SpecificProductId,
	s.StockId, s.[Name] StockName,
	up.UniqueProductId,
	p.ProductId, p.[Name] ProductName, p.Code ProductCode,
	cp.ColorProductId, cp.[Name] ColorProductName, cp.Code ColorProductCode,
	d.DecorId, d.[Name] DecorName, d.Code DecorCode,
	ps.ProductSizeId, ps.[Name] ProductSizeName
	FROM SpecificProductStock sps
	JOIN Stock s ON s.StockId = sps.StockId
	JOIN SpecificProduct sp ON sp.SpecificProductId = sps.SpecificProductId
	JOIN UniqueProduct up ON up.UniqueProductId = sp.UniqueProductId
	JOIN Product p ON p.ProductId = up.ProductId
	JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId
	JOIN Decor d ON d.DecorId = up.DecorId
	JOIN ProductSize ps ON ps.ProductSizeId = sp.ProductSizeId
	WHERE (@IsMainStock IS NULL OR s.IsMainStock = @IsMainStock)
	AND sps.DeletedBy IS NULL
	AND s.StockId IN (SELECT TRY_CONVERT(BIGINT, RTRIM(value)) FROM STRING_SPLIT(@StockIds, ',') WHERE RTRIM(value) != '')
	ORDER BY sps.CurrentCount ASC