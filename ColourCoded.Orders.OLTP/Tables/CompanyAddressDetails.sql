
CREATE TABLE [dbo].[CompanyAddressDetails](
	[AddressDetailId] [int] IDENTITY(1,1) NOT NULL,
	[AddressLine1] [varchar](255) NOT NULL,
	[AddressLine2] [varchar](255) NOT NULL,
	[AddressType] [varchar](255) NOT NULL,
	[City] [varchar](255) NOT NULL,
	[Country] [varchar](255) NOT NULL,
	[PostalCode] [varchar](255) NOT NULL,
	[CompanyProfileId] [int] NOT NULL,
	[CreateUser] [varchar](255) NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[UpdateUser] [varchar](255) NULL,
	[UpdateDate] [datetime2](7) NULL,
) 
GO


