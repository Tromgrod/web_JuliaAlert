CREATE PROCEDURE [dbo].[Translation_Populate]
AS
	BEGIN
		SET NOCOUNT ON;

		SELECT TranslationId,Alias,[Language],[Text] FROM Translation
		
	END