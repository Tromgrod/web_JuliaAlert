CREATE PROCEDURE [dbo].[Generic_Cancel](
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
		UPDATE ['+@TableName+']
		SET DeletedBy = @CancelUserID
		FROM ['+@TableName+'] t
		INNER JOIN dbo.Split(@ItemIDs,'';'') ids ON t.'+@TableName+'ID=ids.value
		'
			
		SET @param_string = N'@ItemIDs varchar(2000),@CancelUserID int';
		PRINT @dynamic_sql
		EXEC sp_executesql @dynamic_sql, @param_string,@ItemIDs,@CancelUserID
	END