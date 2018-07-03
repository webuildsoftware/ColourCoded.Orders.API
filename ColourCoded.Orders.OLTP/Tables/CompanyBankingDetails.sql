
CREATE TABLE [dbo].[CompanyBankingDetails](
	[BankingDetailId] [int] IDENTITY(1,1) NOT NULL,
	[BankName] [varchar](255) NOT NULL,
	[BranchCode] [varchar](255) NOT NULL,
	[AccountType] [varchar](255) NOT NULL,
	[AccountNo] [varchar](255) NOT NULL,
	[AccountHolder] [varchar](255) NOT NULL,
	[CompanyProfileId] [int] NOT NULL,
	[CreateUser] [varchar](255) NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[UpdateUser] [varchar](255) NULL,
	[UpdateDate] [datetime2](7) NULL,
) 
GO


