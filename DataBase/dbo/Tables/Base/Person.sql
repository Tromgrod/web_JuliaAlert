CREATE TABLE [dbo].[Person] (
    [PersonId]    BIGINT         IDENTITY (1, 1) NOT NULL,
    [FirstName]   NVARCHAR (100) NOT NULL,
    [LastName]    NVARCHAR (100) NOT NULL,
    [Email]       NVARCHAR (50)  NULL,
    [SexId]       BIGINT         NOT NULL,
    [Phone]       NVARCHAR (50)  NULL,
    [DeletedBy]   BIGINT         NULL,
    [CreatedBy]   BIGINT         NOT NULL,
    [DateCreated] DATETIME       NOT NULL,
    [DateUpdated] DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([PersonId] ASC),
    CONSTRAINT [FK_Person_Sex] FOREIGN KEY ([SexId]) REFERENCES [dbo].[Sex] ([SexId])
);


