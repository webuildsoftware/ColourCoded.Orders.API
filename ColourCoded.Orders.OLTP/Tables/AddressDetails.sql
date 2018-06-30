CREATE TABLE [dbo].[AddressDetails]
(
  [AddressDetailId] INT IDENTITY(1, 1) PRIMARY KEY,
  [AddressLine1] VARCHAR(255) NOT NULL,
  [AddressLine2] VARCHAR(255) NOT NULL,
  [AddressType] VARCHAR(255) NOT NULL,
  [City] VARCHAR(255) NOT NULL,
  [Country] VARCHAR(255) NOT NULL,
  [PostalCode] VARCHAR(255) NOT NULL,
  [CustomerId] INT NOT NULL,
  [CreateUser] VARCHAR(255) NOT NULL,
  [CreateDate] DATETIME2 NOT NULL,
  [UpdateUser] VARCHAR(255) NULL,
  [UpdateDate] DATETIME2 NULL,
)