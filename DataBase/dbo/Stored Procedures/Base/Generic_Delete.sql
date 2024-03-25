CREATE PROCEDURE [dbo].[Generic_Delete](
	@TableName nvarchar(400),
	@ItemIDs varchar(2000),
	@CancelUserID int = null,
	@Type tinyint = null,
	@Comment varchar(1000) = null,
	@LogRevisions bit = 0,
	@BOName nvarchar(100) = null,
	@Json varchar(MAX) = null
)
AS
	
	BEGIN
		SET NOCOUNT ON;

		DECLARE @dynamic_sql nvarchar(MAX)
		DECLARE @param_string nvarchar(MAX)
		
		SET @dynamic_sql =' 
		DELETE FROM ['+@TableName+']
		FROM  ['+@TableName+'] t
		INNER JOIN dbo.Split(@ItemIDs,'';'') ids ON t.'+@TableName+'ID=ids.value
		'
			
		SET @param_string = N'@ItemIDs varchar(2000)';
		PRINT @dynamic_sql
		EXEC sp_executesql @dynamic_sql, @param_string, @ItemIDs
		
		IF @LogRevisions=1
		 BEGIN
			INSERT INTO BORevision([Table],BOId,BOName,[Date],UserId,Comment,Json,[Type])
			SELECT @TableName,CAST(ids.Value as bigint),@BOName,GETDATE(),@CancelUserID,@Comment,@Json,@Type
			FROM dbo.Split(@ItemIDs,';') ids
			WHERE CAST(ids.Value as bigint)<>0
		 END

	END