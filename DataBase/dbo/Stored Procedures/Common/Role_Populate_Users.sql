CREATE PROCEDURE [dbo].[Role_Populate_Users]
AS
	SELECT r.RoleId, r.[Name], r.Color, COUNT(u.UserId) as [Count]
	FROM [User] u
	JOIN [Role] r ON r.RoleId = u.RoleId
	WHERE u.DeletedBy IS NULL AND u.[Enabled] = 1 AND r.DeletedBy IS NULL
	GROUP BY r.RoleId, r.Name, r.Color