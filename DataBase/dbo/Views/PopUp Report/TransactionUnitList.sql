CREATE VIEW [dbo].[TransactionUnitList]
AS
	SELECT tr.TransactionId, tr.TransactionNumber, cl.ClientId, tr.PayMethodId, tr.TransactionTime, tr.[Sum] TransactionSum
	FROM [Client] cl
	JOIN [Transaction] tr ON tr.ClientId = cl.ClientId AND tr.DeletedBy Is NULL
	WHERE cl.DeletedBy IS NULL