CREATE PROCEDURE [dbo].[User_Populate]
	@UserID bigint
AS
	SELECT u.UserId,u.Login,u.Timeout,u.UniqueId,u.Permission,u.RoleId,r.Name as RoleName, r.Permission as RolePermissions, u.Password, u.PersonId
			,u.DateCreated,u.LastLogin,u.CreatedBy,p.FirstName as PersonFirstName,p.LastName as PersonLastName,p.Email as PersonEmail
	FROM [User] u
	JOIN [Person] p ON p.PersonId=u.PersonId
	JOIN [Role] r ON r.RoleId=u.RoleId
	WHERE u.UserId=@UserID 