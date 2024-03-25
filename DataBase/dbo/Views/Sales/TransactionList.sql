CREATE VIEW [dbo].[TransactionList]
AS 
	SELECT cl.ClientId, tr.PayMethodId, ISNULL(tr.[Sum], 0) TransactionSum, tr.TransactionTime
	FROM [Client] cl
	JOIN [Transaction] tr ON tr.ClientId = cl.ClientId AND tr.DeletedBy Is NULL
	WHERE cl.DeletedBy IS NULL