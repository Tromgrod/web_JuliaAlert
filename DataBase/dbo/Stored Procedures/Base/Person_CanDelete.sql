CREATE PROCEDURE [dbo].[Person_CanDelete]
(	
	@ids nvarchar(MAX)
)
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT u.Login
	FROM Person p
	INNER JOIN [User] u ON u.PersonId=p.PersonId
	INNER JOIN dbo.Split(@ids,';') ids ON ids.Value=p.PersonId

END