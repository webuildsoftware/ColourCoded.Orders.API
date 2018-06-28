CREATE TABLE [dbo].[ContactPersons]
(
  [ContactId] INT IDENTITY(1, 1) PRIMARY KEY,
  [ContactName] VARCHAR(255) NOT NULL,
  [ContactNo] VARCHAR(255) NOT NULL,
  [EmailAddress] VARCHAR(255) NULL,
  [CustomerId] INT NULL,
  [CreateUser] VARCHAR(255) NOT NULL,
  [CreateDate] DATETIME2 NOT NULL,
  [UpdateUser] VARCHAR(255) NULL,
  [UpdateDate] DATETIME2 NULL,
)
