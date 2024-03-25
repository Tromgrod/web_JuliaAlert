CREATE PROCEDURE [dbo].[BORevisions_Populate](
	@Table nvarchar(1000),
	@Id bigint
)
AS
	BEGIN
		SET NOCOUNT ON;

		SELECT bor.BORevisionId,bor.[Table],bor.BOId,bor.BOName,bor.[Date],bor.UserId,bor.[Type],bor.Comment,bor.Json,
		u.UserId,u.Login,u.Timeout,u.UniqueId,u.Permission, u.Password,u.DateCreated,u.LastLogin,u.CreatedBy
		,r.RoleId, r.Name as RoleName, r.Permission as RolePermission
		,ug.GraphicId AS ImageId,ug.BOName AS ImageBOName,ug.Name AS ImageName,ug.Ext AS ImageExt
		,rg.GraphicId AS AvatarId,rg.BOName AS AvatarBOName,rg.Name AS AvatarName,rg.Ext AS AvatarExt
		FROM BORevision bor
		LEFT JOIN [User] u ON u.UserId=bor.UserId
		INNER JOIN [Role] r ON r.RoleId=u.RoleId
		LEFT JOIN [Graphic] ug ON ug.GraphicId=u.ImageId
		LEFT JOIN [Graphic] rg ON rg.GraphicId=r.AvatarId
		WHERE bor.[Table]=@Table AND bor.BOId=@Id
		ORDER BY bor.[Date] desc

	END