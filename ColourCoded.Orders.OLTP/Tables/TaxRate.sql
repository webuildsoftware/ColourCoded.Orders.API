CREATE TABLE [dbo].[TaxRates](
  [TaxRateId] [int] identity(1, 1) NOT NULL,
  [TaxCode] [varchar](255) NOT NULL,
  [Rate] [decimal](19, 4) NOT NULL,
  [StartDate] [datetime2] NOT NULL,
  [EndDate] [datetime2] NOT NULL,
  [CreateUser] [varchar](255) NOT NULL,
  [CreateDate] [datetime2] NOT NULL,
  [UpdateUser] [varchar](255) NULL,
  [UpdateDate] [datetime2] NULL,
) 
GO