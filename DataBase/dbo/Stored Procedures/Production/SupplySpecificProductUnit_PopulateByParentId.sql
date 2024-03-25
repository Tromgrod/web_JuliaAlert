CREATE PROCEDURE [dbo].[SupplySpecificProductUnit_PopulateByParentId]
	@SupplySpecificProductId bigint
AS
	SELECT sspu.*,
	sp.UniqueProductId,
	p.ProductId, cp.ColorProductId, d.DecorId,
	p.[Name] AS ProductName, cp.[Name] AS ColorProductName, d.[Name] AS DecorName,
	cp.Code AS ColorProductCode, d.Code AS DecorCode, p.Code AS ProductCode,
	ps.ProductSizeId, ps.[Name] ProductSizeName
	FROM SupplySpecificProductUnit sspu
	JOIN SupplySpecificProduct ssp ON ssp.SupplySpecificProductId = sspu.SupplySpecificProductId AND ssp.DeletedBy IS NULL
	JOIN SpecificProduct sp ON sp.SpecificProductId = sspu.SpecificProductId AND sp.DeletedBy IS NULL
	JOIN UniqueProduct up ON up.UniqueProductId = sp.UniqueProductId
	JOIN Product p ON p.ProductId = up.ProductId
	JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId
	JOIN Decor d ON d.DecorId = up.DecorId
	JOIN ProductSize ps ON ps.ProductSizeId = sp.ProductSizeId
	WHERE sspu.DeletedBy IS NULL
	AND sspu.SupplySpecificProductId = @SupplySpecificProductId
	ORDER BY sspu.DateCreated DESC