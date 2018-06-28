CREATE TABLE [dbo].[Customers]
(
  [CustomerId] INT IDENTITY(1, 1) PRIMARY KEY,
  [CustomerName] VARCHAR(255) NOT NULL,
  [CustomerDetails] VARCHAR(255) NULL,
  [ContactNo] VARCHAR(255) NOT NULL,
  [AccountNo] VARCHAR(255) NULL,
  [MobileNo] VARCHAR(255) NULL,
  [EmailAddress] VARCHAR(255) NULL,
  [CompanyProfileId] INT NULL,
  [CreateUser] VARCHAR(255) NOT NULL,
  [CreateDate] DATETIME2 NOT NULL,
  [UpdateUser] VARCHAR(255) NULL,
  [UpdateDate] DATETIME2 NULL,
)

