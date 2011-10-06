USE [LRIT]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[ContractingGoverment_Exclusion]') AND parent_object_id = OBJECT_ID(N'[dbo].[Exclusion]'))
ALTER TABLE [dbo].[Exclusion] DROP CONSTRAINT [ContractingGoverment_Exclusion]
GO

USE [LRIT]
GO

/****** Object:  Table [dbo].[Exclusion]    Script Date: 05/26/2010 14:53:45 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exclusion]') AND type in (N'U'))
DROP TABLE [dbo].[Exclusion]
GO

USE [LRIT]
GO

/****** Object:  Table [dbo].[Exclusion]    Script Date: 05/26/2010 14:53:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Exclusion](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ContractingGoverment] [int] NOT NULL,
	[exclusionID] [varchar](50) NOT NULL,
	[ExcludedContractingGovernmentID] [varchar](50) NOT NULL,
	[FromDate] [datetime] NOT NULL,
	[ToDate] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.Exclusion] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Exclusion]  WITH CHECK ADD  CONSTRAINT [ContractingGoverment_Exclusion] FOREIGN KEY([ContractingGoverment])
REFERENCES [dbo].[ContractingGoverment] ([Id])
GO

ALTER TABLE [dbo].[Exclusion] CHECK CONSTRAINT [ContractingGoverment_Exclusion]
GO


