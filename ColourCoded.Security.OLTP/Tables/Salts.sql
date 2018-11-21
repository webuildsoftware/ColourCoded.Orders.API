﻿CREATE TABLE [dbo].[Salts]
(
  [Id] INT IDENTITY(1,1) PRIMARY KEY,
  [Value] VARCHAR(255) NOT NULL,
  [Active] BIT NOT NULL,
  [CreateDate] DATETIME2 NOT NULL,
  [CreateUser] VARCHAR(255) NOT NULL,
  [UpdateDate] DATETIME2 NULL,
  [UpdateUser] VARCHAR(255) NULL,
)
