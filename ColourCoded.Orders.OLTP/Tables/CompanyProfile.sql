CREATE TABLE [dbo].[CompanyProfiles](
  [CompanyProfileId] [int] identity(1, 1) NOT NULL,
  [DisplayName] [varchar](255) NOT NULL,
  [LegalName] [varchar](255) NOT NULL,
  [RegistrationNo] [varchar](255) NULL,
  [TaxReferenceNo] [varchar](255) NULL,
  [EmailAddress] [varchar](255) NULL,
  [TelephoneNo] [varchar](255) NULL,
  [FaxNo] [varchar](255) NULL,
  [OrderNoSeed] [int] default(1) NOT NULL,
  [CreateUser] [varchar](255) NOT NULL,
  [CreateDate] [datetime2] NOT NULL,
  [UpdateUser] [varchar](255) NULL,
  [UpdateDate] [datetime2] NULL,
) 
GO