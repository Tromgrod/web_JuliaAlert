CREATE PROCEDURE [dbo].[User_Populate_Latests]
	@LastestId bigint = null
AS
	SELECT TOP 20 u.UserId, u.[Login], u.UpdatedById, u.UniqueId, u.DateCreated, u.LastLogin,
	r.RoleId, r.[Name] AS RoleName, r.Color AS RoleColor,
	p.PersonId, p.FirstName AS PersonFirstName, p.LastName AS PersonLastName
	FROM [User] u
	JOIN Person p ON p.PersonId = u.PersonId
	JOIN [Role] r ON r.RoleId = u.RoleId
	WHERE u.DeletedBy IS NULL AND u.[Enabled] = 1
	AND (@LastestId IS NULL OR u.UserId > @LastestId)
	ORDER BY u.LastLogin DESC