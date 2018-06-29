CREATE TABLE [dbo].[OrderHeads]
(
  [OrderId] INT IDENTITY(1, 1) PRIMARY KEY,
  [OrderNo] VARCHAR(255) NOT NULL,
  [SubTotal] MONEY NOT NULL,
  [VatTotal] MONEY NOT NULL,
  [DiscountTotal] MONEY NOT NULL,
  [OrderTotal] MONEY NOT NULL,
  [VatRate] DECIMAL(19,4) NOT NULL,
  [CompanyProfileId] [int] DEFAULT(0) NOT NULL,
  [CustomerId] INT DEFAULT(0) NOT NULL,
  [ContactId] INT DEFAULT(0) NOT NULL,
  [CreateDate] DATETIME2 NOT NULL,
  [CreateUser] VARCHAR(255) NOT NULL,
  [UpdateUser] VARCHAR(255) NULL,
  [UpdateDate] DATETIME2 NULL,
)

