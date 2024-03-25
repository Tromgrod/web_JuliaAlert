CREATE PROCEDURE [dbo].[TypeProduct_PopulateByGroupProduct]
	@GroupProductId bigint
AS
	SELECT tp.TypeProductId, tp.[Name] 
	FROM TypeProduct tp
	WHERE @GroupProductId = 0 OR tp.GroupProductId = @GroupProductId