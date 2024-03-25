CREATE PROCEDURE [dbo].[Generic_Insert](
	@ParameterValues nvarchar(MAX)=null,
	@InsertToParameterValues nvarchar(MAX)=null,
	@TableName nvarchar(400),
	@InsertQuery nvarchar(MAX)=null,
	@InsertParamValues nvarchar(MAX)=null,
	@InsertParams nvarchar(MAX),
	@InsertValuesQuery nvarchar(MAX),
	@TranslationUpdate nvarchar(MAX)=null,
	@insertChildTables nvarchar(MAX)=null,
	@CreatedBy bigint,
	@Comment nvarchar(1000) = null,
	@Json nvarchar(MAX) = null,
	@LogRevisions bit = 0,
	@BOName nvarchar(100) = null,
	@Type tinyint = null
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
				+@InsertToParameterValues
			
			SET @param_string = N'@InsertParams nvarchar(MAX),@InsertParamValues nvarchar(MAX),';
		END
		
		SET @dynamic_sql = @dynamic_sql + ' 
		DECLARE @ID bigint
		
		INSERT INTO ['+@TableName+']('+@InsertQuery+',CreatedBy,DateCreated)
		VALUES ('+@InsertValuesQuery+',@CreatedBy,GETDATE())
		
		SET @ID =  CAST(SCOPE_IDENTITY() AS bigint)
		'
		IF @TranslationUpdate IS NOT NULL
		BEGIN
		SET @dynamic_sql =@dynamic_sql +' 
		
		UPDATE ['+@TableName+']
		SET '+@TranslationUpdate+'
		WHERE '+@TableName+'ID=@ID	
		'		
		END
				
		IF @ParameterValues IS NOT NULL		
		BEGIN
			SET @dynamic_sql =@dynamic_sql+' 
			DROP TABLE #ParameterValues
			'
		END
		
		IF @LogRevisions=1
		 BEGIN
			SET @dynamic_sql =@dynamic_sql +' 
			INSERT INTO BORevision([Table],BOId,BOName,[Date],UserId,Comment,Json,[Type])
			VALUES(@TableName,@ID,@BORevisionName,GETDATE(),@CreatedBy,@Comment,@Json,@Type)
			'
		 END

		IF @insertChildTables IS NOT NULL		
		BEGIN
			SET @dynamic_sql =@dynamic_sql+@insertChildTables
		END

		SET @dynamic_sql =@dynamic_sql +' 
		SELECT @ID
		'
		
		SET @param_string = @param_string + N'@CreatedBy bigint,@Comment nvarchar(1000),@Json nvarchar(MAX),@Type tinyint,@TableName nvarchar(400),@BORevisionName nvarchar(100)'
		PRINT @dynamic_sql
		EXEC sp_executesql @dynamic_sql, @param_string,@InsertParams,@InsertParamValues,@CreatedBy,@Comment,@Json,@Type,@TableName,@BOName
		

	END

		
