CREATE PROCEDURE [dbo].[Translation_Update]
(
	@Alias nvarchar(50)='',
	@Language nvarchar(10)='',
	@Text nvarchar(MAX)=''
)
AS
	BEGIN
		SET NOCOUNT ON;

		IF NOT EXISTS(SELECT Alias FROM Translation WHERE Alias=@Alias AND [Language]=@Language )
			BEGIN
				INSERT INTO Translation(Alias,[Language],[Text],DateCreated)
				VALUES(@Alias,@Language,@Text, GETDATE())			
			END
		ELSE
			BEGIN
				UPDATE Translation
				SET
					[Text]=@Text,
					DateUpdated = GETDATE()
				WHERE 
					Alias=@Alias AND [Language]=@Language
			END		
	END