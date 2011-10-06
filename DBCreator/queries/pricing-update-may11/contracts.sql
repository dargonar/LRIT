USE [LRIT]
GO

/****** Object:  Table [dbo].[Contract]    Script Date: 06/09/2011 20:37:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Contract](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[lritid] [varchar](8) NOT NULL,
	[name] [varchar](128) NOT NULL,
	[minimun] [varchar](50) NOT NULL,
	[period] [varchar](50) NOT NULL,
	[lastInvoice] [date] NULL,
	[credit_balance] [decimal](18, 4) NOT NULL,
	[lastInvoiceRecv] [date] NULL,
	[debit_balance] [decimal](18, 4) NOT NULL,
 CONSTRAINT [PK_Contractx] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Contract] ADD  CONSTRAINT [DF_Contractx_period]  DEFAULT ('') FOR [period]
GO

ALTER TABLE [dbo].[Contract] ADD  CONSTRAINT [DF_Contractx_deuda]  DEFAULT ((0)) FOR [credit_balance]
GO

ALTER TABLE [dbo].[Contract] ADD  CONSTRAINT [DF_Contract_saldo]  DEFAULT ((0)) FOR [debit_balance]
GO


