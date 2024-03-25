CREATE PROCEDURE [dbo].[Generic_PopulateReport](
	@ParameterValues nvarchar(MAX)=null,
	@InsertToParameterValues nvarchar(MAX)=null,
	@SearchParams nvarchar(MAX)=null,
	@SearchParamValues nvarchar(MAX)=null,
	@SelectQuery nvarchar(MAX),
	@JoinQuery nvarchar(MAX)='',
	@SearchQuery nvarchar(MAX)=null,
	@SortBy nvarchar(MAX)='',
	@GroupSort nvarchar(MAX)=null,
	@AdvancedFilter nvarchar(MAX)='',
	@PreinitVar nvarchar(MAX)='',
	@SimpleSearchQuery nvarchar(MAX)='',
	@AdditionalJoin nvarchar(MAX)='',
	@AdditionalGroup nvarchar(MAX)='',
	@AfterPaginAdditionalQuery nvarchar(MAX)='*',
	@sUser bigint=null,
	@sSearch nvarchar(100)='',
	@ShowCanceled bit=0,
	@PagingStart int,
	@PagingLen int,
	@CustomPage bit = 0,
	@CustomCountDistinct nvarchar(MAX)='*',
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
		
		IF @CustomCountDistinct != '*'
			BEGIN
				SET @CustomCountDistinct = 'DISTINCT ' + @CustomCountDistinct
			END

		DECLARE @SortByQuery nvarchar(MAX)

		IF @SortBy=''
			BEGIN
				SET @SortByQuery ='
				Order by 1 desc
				'
			END
		ELSE
			BEGIN
				SET @SortByQuery = @SortBy 
			END
		
		DECLARE @condition_string nvarchar(MAX),@agr nvarchar(MAX)
		
		SET @condition_string=''
		SET @agr=''
			
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

		SET @dynamic_sql = @dynamic_sql+' 
		SELECT * FROM (
		SELECT '+@AfterPaginAdditionalQuery+' 
		FROM ( 
		SELECT * FROM ('

		SET @dynamic_sql = @dynamic_sql+' 
		SELECT '+@SelectQuery

		IF @CustomPage = 0
			BEGIN
				SET @dynamic_sql = @dynamic_sql + ',row_number() OVER('+@SortByQuery+') as pageNum '
			END
		
		SET @dynamic_sql = @dynamic_sql +
		' FROM ['+@TableName+'] t 
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
		
		SET @dynamic_sql =@dynamic_sql + '
		) t'

		IF @PagingLen>0 AND @CustomPage = 0
			BEGIN
				SET @dynamic_sql =@dynamic_sql + '
				WHERE pageNum-1 between @PagingStart and (@PagingStart+@PagingLen-1) 
				'
			END
			
		SET @dynamic_sql =@dynamic_sql + ') t 
		' +@AdditionalJoin

		SET @dynamic_sql =@dynamic_sql + @AdditionalGroup + ') t
		'

		IF @GroupSort IS NOT NULL
			BEGIN
				SET @dynamic_sql =@dynamic_sql + @GroupSort
			END

		SET @dynamic_sql =@dynamic_sql + @SortByQuery

		IF @PagingLen>0 AND @CustomPage = 0
			BEGIN
				SET @dynamic_sql =  @dynamic_sql+' 

				SELECT @TotalDispalyValues=count(' + @CustomCountDistinct + ')
				FROM ['+@TableName+'] t 
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
				SELECT @TotalValues=count(' + @CustomCountDistinct + ')
				FROM ['+@TableName+'] t 
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