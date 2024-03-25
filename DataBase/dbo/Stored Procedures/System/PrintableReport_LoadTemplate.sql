CREATE PROCEDURE [dbo].[PrintableReport_LoadTemplate](
	@Tag nvarchar(50)
)
AS	
	BEGIN
		
		SELECT TOP 1 pr.PrintableReportId,pr.Name,pr.ReportTemplate,pr.WordTemplate
		FROM PrintableReport pr
		INNER JOIN PrintableReportType prt ON prt.PrintableReportTypeId=pr.PrintableReportTypeId
		WHERE 
		prt.Tag=@Tag 
		AND pr.Enabled=1 
		AND pr.DeletedBy IS NULL

	END