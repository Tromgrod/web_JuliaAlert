CREATE PROCEDURE [dbo].[Order_Populate_One]
	@OrderId bigint
AS
	SELECT o.*, o.CreatedBy CreatedById,
	cl.[Index] ClientIndex, cl.[Name] ClientName, cl.[Address] ClientAddress, cl.Discount ClientDiscount,
	co.CountriesId, co.[Name] CountriesName,
	s.StateId, s.[Name] StateName, s.ShortName StateShortName,
	ci.CityId, ci.[Name] CityName,
	os.[Name] OrderStateName, os.Color OrderStateColor,
	c.CurrencyId, c.[Name] CurrencyName
	FROM [Order] o
	LEFT JOIN Client cl ON cl.ClientId= o.ClientId
	LEFT JOIN Countries co ON co.CountriesId = cl.CountriesId
	LEFT JOIN [State] s ON s.StateId = cl.StateId
	LEFT JOIN City ci ON ci.CityId = cl.CityId
	JOIN OrderState os ON os.OrderStateId = o.OrderStateId
	JOIN SalesChannel sc ON sc.SalesChannelId = o.SalesChannelId
	JOIN Currency c ON c.CurrencyId = sc.CurrencyId
	WHERE o.DeletedBy IS NULL
	AND o.OrderId = @OrderId

	SELECT pfo.*, up.UniqueProductId,
	p.ProductId, p.[Name] ProductName, p.Code ProductCode,
	cp.ColorProductId, cp.[Name] ColorProductName,
	c.CompoundId, c.[Name] CompoundName,
	tp.TypeProductId, tp.[Name] TypeProductName,
	ps.ProductSizeId, ps.[Name] ProductSizeName
	FROM ProductForOrder pfo
	JOIN SpecificProduct sp ON sp.SpecificProductId = pfo.SpecificProductId
	JOIN UniqueProduct up ON up.UniqueProductId = sp.UniqueProductId
	JOIN Product p ON p.ProductId = up.ProductId
	JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId
	LEFT JOIN TypeProduct tp ON tp.TypeProductId = p.TypeProductId
	LEFT JOIN GroupProduct gp ON gp.GroupProductId = tp.GroupProductId
	LEFT JOIN Compound c ON c.CompoundId = up.CompoundId
	JOIN ProductSize ps ON ps.ProductSizeId= sp.ProductSizeId
	WHERE pfo.DeletedBy IS NULL AND pfo.OrderId = @OrderId

	SELECT g.*, up.UniqueProductId,
	p.ProductId, p.[Name] ProductName, p.Code ProductCode,
	cp.ColorProductId, cp.[Name] ColorProductName,
	c.CompoundId, c.[Name] CompoundName,
	tp.TypeProductId, tp.[Name] TypeProductName
	FROM Gift g
	JOIN SpecificProduct sp ON sp.SpecificProductId = g.SpecificProductId
	JOIN UniqueProduct up ON up.UniqueProductId = sp.UniqueProductId
	JOIN Product p ON p.ProductId = up.ProductId
	JOIN ColorProduct cp ON cp.ColorProductId = up.ColorProductId
	JOIN TypeProduct tp ON tp.TypeProductId = p.TypeProductId
	JOIN GroupProduct gp ON gp.GroupProductId = tp.GroupProductId
	LEFT JOIN Compound c ON c.CompoundId = up.CompoundId
	WHERE g.DeletedBy IS NULL AND g.OrderId = @OrderId