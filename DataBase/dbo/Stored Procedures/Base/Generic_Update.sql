CREATE PROCEDURE [dbo].[Generic_Update](
	@ParameterValues nvarchar(MAX)=null,
	@UpdateToParameterValues nvarchar(MAX)=null,
	@TableName nvarchar(400),
	@UpdateQuery nvarchar(MAX)=null,
	@UpdateParamValues nvarchar(MAX)=null,
	@UpdateParams nvarchar(MAX)=null,
	@TranslationUpdate nvarchar(MAX)=null,
	@updateChildTables nvarchar(MAX)=null,
	@ID bigint,
	@Comment nvarchar(1000) = null,
	@Json nvarchar(MAX) = null,
	@Type tinyint = null,
	@LogRevisions bit = 0,
	@BOName nvarchar(100) = null,
	@UpdatedBy bigint	
)
AS
	
	BEGIN
		SET NOCOUNT ON;	
		
		DECLARE @dynamic_sql nvarchar(MAX)
		DECLARE @param_string nvarchar(MAX)
		SET @param_string =''
		SET @dynamic_sql=''
		IF @ParameterValues IS NOT NULL
		BEGIN
			SET @dynamic_sql =' 
			CREATE TABLE #ParameterValues
			(
				ParamName varchar(300),
				'
				+@ParameterValues+
				'
			)
			'
				+@UpdateToParameterValues
			
		END
		
		
		IF @UpdateParams IS NOT NULL
		BEGIN
		SET @dynamic_sql = @dynamic_sql + ' 
		
		UPDATE ['+@TableName+']
		SET '+@UpdateQuery+'
		WHERE '+@TableName+'ID=@ID
		
		'
		END
		
		IF @TranslationUpdate IS NOT NULL
		BEGIN
		SET @dynamic_sql =@dynamic_sql +' 
		
		SELECT '+@TranslationUpdate+' FROM ['+@TableName+'] WHERE '+@TableName+'ID=@ID'
				
		END
				
		IF @ParameterValues IS NOT NULL		
		BEGIN
			SET @dynamic_sql =@dynamic_sql+' 
			DROP TABLE #ParameterValues
			'
		END
		
		IF @updateChildTables IS NOT NULL		
		BEGIN
			SET @dynamic_sql =@dynamic_sql+@updateChildTables
		END

		SET @param_string = @param_string + N'@UpdateParams nvarchar(MAX),@UpdateParamValues nvarchar(MAX),@ID bigint'
		PRINT @dynamic_sql
		EXEC sp_executesql @dynamic_sql, @param_string,@UpdateParams,@UpdateParamValues,@ID
		
		IF @LogRevisions=1
		 BEGIN
			INSERT INTO BORevision([Table],BOId,BOName,[Date],UserId,Comment,Json,[Type])
			VALUES(@TableName,@ID,@BOName,GETDATE(),@UpdatedBy,@Comment,@Json,@Type)
		 END

	END

		
