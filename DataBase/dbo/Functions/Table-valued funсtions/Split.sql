CREATE FUNCTION [dbo].[Split]
(
	@List nvarchar(MAX),
	@char char(1)
)
RETURNS @ParsedList table
(
	Ident int IDENTITY(1, 1),
	Value nvarchar(MAX)
)
AS
BEGIN
	DECLARE @SeatID nvarchar(MAX), @Pos int

	IF @List IS NULL RETURN	

	SET @List = LTRIM(RTRIM(@List))+ @char
	SET @Pos = CHARINDEX(@char, @List, 1)

	IF REPLACE(@List, @char, '') <> ''
	BEGIN
		WHILE @Pos > 0
		BEGIN
			SET @SeatID = LTRIM(RTRIM(LEFT(@List, @Pos - 1)))
			
			INSERT INTO @ParsedList (Value) 			
			VALUES (@SeatID)

			SET @List = RIGHT(@List, LEN(@List) - @Pos)
			SET @Pos = CHARINDEX(@char, @List, 1)

		END
	END	
	RETURN
END
