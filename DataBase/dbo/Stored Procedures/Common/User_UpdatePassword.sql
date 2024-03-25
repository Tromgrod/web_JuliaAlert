CREATE PROCEDURE [dbo].[User_UpdatePassword]
(
	@UserId	bigint,
	@Password nvarchar(50)
)
AS
BEGIN
	UPDATE [User] SET [Password] = @Password  WHERE UserId = @UserId
END