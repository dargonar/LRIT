USE [LRIT]
GO

/****** Object:  Table [dbo].[Price]    Script Date: 06/09/2011 20:41:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Price](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[effectiveDate] [date] NOT NULL,
	[issueDate] [date] NULL,
	[dataCentreID] [varchar](8) NOT NULL,
	[currency] [varchar](8) NOT NULL,
	[PeriodicRateChange] [decimal](18, 4) NOT NULL,
	[Poll] [decimal](18, 4) NOT NULL,
	[PositionReport] [decimal](18, 4) NOT NULL,
	[ArchivePositionReport] [decimal](18, 4) NOT NULL,
 CONSTRAINT [PK_Pricess_1] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Price] ADD  DEFAULT ((0)) FOR [PeriodicRateChange]
GO

ALTER TABLE [dbo].[Price] ADD  DEFAULT ((0)) FOR [Poll]
GO

ALTER TABLE [dbo].[Price] ADD  DEFAULT ((0)) FOR [PositionReport]
GO

ALTER TABLE [dbo].[Price] ADD  DEFAULT ((0)) FOR [ArchivePositionReport]
GO


