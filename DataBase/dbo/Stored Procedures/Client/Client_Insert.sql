CREATE PROCEDURE [dbo].[Client_Insert]
	@Name NVarChar(100),
    @CountriesId bigint = null,
    @StateId bigint = null,
    @CityId bigint = null,
    @Index NVarChar(100) = null,
    @Address NVarChar(200) = null,
    @Phone NVarChar(100) = null,
    @Comment NVarChar(MAX) = null,
    @Discount Int,
    @Birthday DateTime = null,
    @Email NVarChar(100) = null,
    @CreatedBy bigint
AS
	INSERT Client
    VALUES(@Name, @CountriesId, @StateId, @CityId, @Index, @Address, @Phone, @Comment, @Discount, @Birthday, @Email, NULL, @CreatedBy, GETDATE(), NULL)

    SELECT scope_identity()