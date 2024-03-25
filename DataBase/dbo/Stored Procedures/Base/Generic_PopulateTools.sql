CREATE PROCEDURE [dbo].[Generic_PopulateTools](
	@ParameterValues nvarchar(MAX)=null,
	@InsertToParameterValues nvarchar(MAX)=null,
	@SearchParams nvarchar(MAX)=null,
	@SearchParamValues nvarchar(MAX)=null,
	@SelectQuery nvarchar(MAX),
	@JoinQuery nvarchar(MAX)='',
	@SearchQuery nvarchar(MAX)=null,
	@SortBy nvarchar(MAX)='',
	@AdvancedFilter nvarchar(MAX)='',
	@PreinitVar nvarchar(MAX)='',
	@SimpleSearchQuery nvarchar(MAX)='',
	@AdditionalJoin nvarchar(MAX)='',
	@AfterPaginAdditionalQuery nvarchar(MAX)='',
	@sUser bigint=null,
	@sSearch nvarchar(100)='',
	@ShowCanceled bit=0,
	@PagingStart int,
	@PagingLen int,
	@TableName nvarchar(400),
	@TotalValues bigint OUTPUT,
	@TotalDispalyValues bigint OUTPUT,
	@sID bigint=null
)
AS
	
	BEGIN

		DECLARE @dynamic_sql nvarchar(MAX)
		DECLARE @param_string nvarchar(MAX)
		
		SET @dynamic_sql=''
		SET @param_string=''
				
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
			
		END
		
		DECLARE @SortByQuery nvarchar(MAX)

		IF @SortBy=''
			BEGIN
				SET @SortByQuery ='
				Order by t.'+@TableName+'ID asc
				'
			END
		ELSE
			BEGIN
				SET @SortByQuery = @SortBy 
			END
		
		DECLARE @condition_string nvarchar(MAX),@agr nvarchar(MAX)
		
		SET @condition_string=''
		SET @agr=''

		IF( @ShowCanceled=0)
		BEGIN
			SET @condition_string=' t.DeletedBy IS NULL '
			SET @agr=' AND '
		END

		IF(@sID IS NOT NULL)
			BEGIN 
				SET @condition_string = @condition_string + @agr +' 
				t.'+@TableName+'ID=@sID '
				SET @agr=' AND '
			END

		IF(@sUser IS NOT NULL)
			BEGIN 
				SET @condition_string = @condition_string + @agr +' 
				t.CreatedBy = '+CAST(@sUser as nvarchar(10))
				SET @agr=' AND '
			END
			
		IF(@agr='')
		BEGIN
			SET @condition_string = ' 1=1 ' + @AdvancedFilter
		END
		ELSE
		BEGIN
			SET @condition_string = @condition_string + @AdvancedFilter
		END
		
		IF(@PreinitVar IS NOT NULL)
			BEGIN 
				SET @dynamic_sql = @dynamic_sql + @PreinitVar
			END

		IF @PagingLen>0
			BEGIN
				SET @dynamic_sql = @dynamic_sql+' 
				SELECT * FROM ('
			END

		SET @dynamic_sql = @dynamic_sql+' 
		SELECT '+@SelectQuery+',t.DateCreated,t.CreatedBy,ut.UserID as CreatedByID,ut.login as CreatedBylogin,row_number() OVER('+@SortByQuery+') as pageNum
		FROM ['+@TableName+'] t 
		LEFT JOIN [User] ut ON ut.UserID=t.CreatedBy
		'
		+@JoinQuery+
		+@AdditionalJoin+
		'
		WHERE '+@condition_string
		
		IF @SearchQuery IS NOT NULL 
		BEGIN
			SET @dynamic_sql =@dynamic_sql +@SearchQuery
		END
		 
		IF @SimpleSearchQuery IS NOT NULL 
		BEGIN
			SET @dynamic_sql =@dynamic_sql +@SimpleSearchQuery
		END
		
		IF @PagingLen>0
			BEGIN
				SET @dynamic_sql =@dynamic_sql + '
				) t
				WHERE pageNum-1 between @PagingStart and (@PagingStart+@PagingLen-1)	
				' 
			END

		SET @dynamic_sql =@dynamic_sql + @SortByQuery 

		IF @PagingLen>0
			BEGIN
				SET @dynamic_sql =  @dynamic_sql+' 

				SELECT @TotalDispalyValues=count(*)
				FROM ['+@TableName+'] t 
				LEFT JOIN [User] ut ON ut.UserID=t.CreatedBy
				'
				+@JoinQuery+
				'
				WHERE '+@condition_string						

				IF @SearchQuery IS NOT NULL 
				BEGIN
					SET @dynamic_sql =@dynamic_sql +@SearchQuery
				END
		 
				IF @SimpleSearchQuery IS NOT NULL 
				BEGIN
					SET @dynamic_sql =@dynamic_sql +@SimpleSearchQuery
				END


				SET @dynamic_sql = @dynamic_sql+' 
				SELECT @TotalValues=count(*)
				FROM ['+@TableName+'] t 
				LEFT JOIN [User] ut ON ut.UserID=t.CreatedBy
				'
				+@JoinQuery+
				'
				WHERE '+@condition_string
		
			END
			
		IF @ParameterValues IS NOT NULL		
		BEGIN
			SET @dynamic_sql =@dynamic_sql+' 
			DROP TABLE #ParameterValues'
		END

		SET @TotalDispalyValues=0
		SET @TotalValues=0

		SET @param_string = @param_string+N'@SearchParams nvarchar(MAX),@SearchParamValues nvarchar(MAX),@sID int,@sSearch nvarchar(100),@PagingStart int,@PagingLen int,@TotalDispalyValues bigint OUTPUT,@TotalValues bigint OUTPUT'
				
		PRINT @dynamic_sql
		EXEC sp_executesql @dynamic_sql, @param_string, @SearchParams,@SearchParamValues,@sID,@sSearch,@PagingStart,@PagingLen,@TotalDispalyValues OUTPUT,@TotalValues OUTPUT
		
	END

		
