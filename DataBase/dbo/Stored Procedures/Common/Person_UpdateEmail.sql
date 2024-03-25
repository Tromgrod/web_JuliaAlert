CREATE PROCEDURE [dbo].[Person_UpdateEmail]
(
    @UserId bigint,
    @Email nvarchar(50)
)
AS
BEGIN

UPDATE [Person] SET Person.Email = @Email
    FROM Person INNER JOIN [User] ON [User].PersonId = Person.PersonId 
    WHERE [User].UserId = @UserId
END
