CREATE PROCEDURE [dbo].[User_CheckLoginForUniqueness]
(	
	@id bigint, 
	@Login nvarchar(200)
)
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @cnt bigint
	SET @cnt = 0
	
	SET @id = COALESCE(@id, 0);
	
	SELECT @cnt = COUNT(*) FROM [User] WHERE [Login] = @Login AND UserID != @id
		
	IF @cnt <= 0
		RETURN 1;
			
	RETURN 0;
END