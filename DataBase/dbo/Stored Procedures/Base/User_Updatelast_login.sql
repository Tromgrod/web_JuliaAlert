CREATE PROCEDURE [dbo].[User_Updatelast_login]
(
	@UserID	bigint
)
AS
BEGIN

	UPDATE [User] 
	SET LastLogin = getDate()  
	WHERE UserId = @UserID

END