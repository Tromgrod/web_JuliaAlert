CREATE PROCEDURE [dbo].[Person_Populate](
	@UserId bigint
)
AS	
	BEGIN
		
		SELECT p.PersonId
		FROM Person p
		INNER JOIN [User] u ON u.PersonId=p.PersonId
		WHERE u.UserId=@UserId
		AND p.DeletedBy iS NULL

	END