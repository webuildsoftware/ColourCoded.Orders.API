CREATE TABLE [dbo].[OrderDetails]
(
  [OrderDetailId] INT IDENTITY(1, 1) PRIMARY KEY,
  [LineNo] INT NOT NULL,
  [Negate] BIT DEFAULT(0) NOT NULL,
  [OrderId] INT NOT NULL,
  [ItemDescription] VARCHAR(255) NOT NULL,
  [UnitPrice] MONEY NOT NULL,
  [Quantity] INT NOT NULL,
  [Discount] MONEY NOT NULL,
  [LineTotal] MONEY NOT NULL,
  [CreateDate] DATETIME2 NOT NULL,
  [CreateUser] VARCHAR(255) NOT NULL,
  [UpdateUser] VARCHAR(255) NULL,
  [UpdateDate] DATETIME2 NULL,
)