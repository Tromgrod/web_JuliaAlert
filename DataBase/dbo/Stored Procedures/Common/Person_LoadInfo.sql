CREATE PROCEDURE [dbo].[Person_LoadInfo]
(
	@UserId bigint
)

AS
	BEGIN
		SET NOCOUNT ON;
		
		exec User_Updatelast_login @UserId

		SELECT p.PersonId ,p.FirstName,p.LastName,p.Email
		FROM [User] u
		INNER JOIN [Person] p ON p.PersonId=u.PersonId
		WHERE u.UserId=@UserId
		
	END
