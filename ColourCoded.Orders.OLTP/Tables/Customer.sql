CREATE TABLE [dbo].[Customers]
(
  [CustomerId] INT IDENTITY(1, 1) PRIMARY KEY,
  [CustomerName] VARCHAR(255) NOT NULL,
  [CustomerAccountNo] VARCHAR(255) NOT NULL,
  [PersonId] INT NULL,
  [CreateUser] VARCHAR(255) NULL,
  [CreateDate] DATETIME2 NULL,
  [UpdateUser] VARCHAR(255) NULL,
  [UpdateDate] DATETIME2 NULL,
)

