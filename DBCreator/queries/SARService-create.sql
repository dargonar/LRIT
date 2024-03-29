USE [LRIT]
GO
/****** Object:  Table [dbo].[SARService]    Script Date: 09/27/2010 11:29:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SARService](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ContractingGovermentId] [int] NOT NULL,
	[LRITid] [varchar](50) NOT NULL,
	[Name] [varchar](512) NULL,
 CONSTRAINT [PK_SARService] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[SARService]  WITH CHECK ADD  CONSTRAINT [FK_SARService_ContractingGoverment1] FOREIGN KEY([ContractingGovermentId])
REFERENCES [dbo].[ContractingGoverment] ([Id])
GO
ALTER TABLE [dbo].[SARService] CHECK CONSTRAINT [FK_SARService_ContractingGoverment1]