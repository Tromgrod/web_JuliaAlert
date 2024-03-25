CREATE TRIGGER [UniqueProduct_INSERT_TRG]
	ON [dbo].[UniqueProduct]
	FOR INSERT
	AS
	BEGIN
		DECLARE @ProductId BIGINT = (SELECT TOP(1) ProductId FROM inserted) 

		DECLARE @CountInserted INT = 
		(SELECT COUNT(*) FROM inserted) 
		* 
		(SELECT DISTINCT COUNT(*) FROM SpecificProduct sp JOIN UniqueProduct up ON up.UniqueProductId = sp.UniqueProductId AND up.ProductId = @ProductId)

		CREATE TABLE #ProductCodes (
		   ProductCode NVarChar(5),
		   [Row] BIGINT
		)
		INSERT INTO #ProductCodes EXEC SpecificProduct_GenerateProductCode @Count = @CountInserted;

		WITH InsertValue AS
		(
			SELECT t.ProductSizeId, t.UniqueProductId, t.CreatedBy, codes.ProductCode 
			FROM (
				SELECT sizes.ProductSizeId, i.UniqueProductId, i.CreatedBy, ROW_NUMBER() OVER(ORDER BY sizes.ProductSizeId ASC) AS [Row] 
				FROM inserted i
				CROSS APPLY (
					SELECT DISTINCT sp.ProductSizeId 
					FROM SpecificProduct sp 
					JOIN UniqueProduct up ON up.UniqueProductId = sp.UniqueProductId AND up.ProductId = @ProductId
				) sizes
			) t
			JOIN #ProductCodes codes ON codes.[Row] = t.[Row]
		)

		INSERT SpecificProduct (UniqueProductId, ProductSizeId, ProductCode, CreatedBy, DateCreated)
		SELECT val.UniqueProductId, val.ProductSizeId, val.ProductCode, val.CreatedBy, GETDATE() 
		FROM InsertValue val

		DROP TABLE #ProductCodes
	END