USE [LRIT]
GO

/****** Object:  Table [dbo].[Invoice]    Script Date: 06/09/2011 20:39:00 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Invoice](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[emitidarecibida] [int] NOT NULL,
	[invoiceNumber] [varchar](64) NOT NULL,
	[contract_id] [int] NOT NULL,
	[dateFrom] [date] NOT NULL,
	[dateTo] [date] NOT NULL,
	[amount] [decimal](18, 4) NOT NULL,
	[currency] [varchar](8) NOT NULL,
	[transfercost] [decimal](18, 4) NOT NULL,
	[interests] [decimal](18, 4) NOT NULL,
	[bankreference] [varchar](64) NULL,
	[notes] [varchar](128) NOT NULL,
	[isueDate] [date] NOT NULL,
	[state] [int] NOT NULL,
 CONSTRAINT [PK_Invoice] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Invoice]  WITH CHECK ADD  CONSTRAINT [FK_Invoice_Contract] FOREIGN KEY([contract_id])
REFERENCES [dbo].[Contract] ([id])
GO

ALTER TABLE [dbo].[Invoice] CHECK CONSTRAINT [FK_Invoice_Contract]
GO

ALTER TABLE [dbo].[Invoice] ADD  CONSTRAINT [DF_Invoice_transfercost]  DEFAULT ((0)) FOR [transfercost]
GO


