CREATE PROCEDURE [dbo].[Person_Populate_ByPermission]
(
	@Permission bigint
)

AS
	BEGIN
		SET NOCOUNT ON;
		
		SELECT p.PersonId ,p.FirstName,p.LastName
		FROM [Person] p
		INNER JOIN [User] u ON p.PersonId=u.PersonId
		INNER JOIN [Role] r ON r.RoleId=u.RoleId
		WHERE r.Permission & @Permission != 0 OR u.Permission & @Permission != 0
		
	END
