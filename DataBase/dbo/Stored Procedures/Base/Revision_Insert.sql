CREATE PROCEDURE [dbo].[Revision_Insert](
	@Table nvarchar(1000),
	@UserId bigint,
	@BOId bigint,
	@BOName nvarchar(200),
	@Comment nvarchar(2000),
	@Date datetime,
	@Json nvarchar(MAX) = null,
	@Type tinyint
)
AS
	BEGIN
		SET NOCOUNT ON;
		
		INSERT INTO BORevision([Table],BOId,BOName,[Date],UserId,Comment,[Type],Json)
		VALUES(@Table,@BOId,@BOName,@Date,@UserId,@Comment,@Type,@Json)

		SELECT CAST(SCOPE_IDENTITY() AS bigint)
	END