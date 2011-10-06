USE [LRIT]
GO
/****** Object:  Table [dbo].[PendingDDPUpdates]    Script Date: 10/19/2010 12:54:44 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[PendingDDPUpdates](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ddpUpdateId] [int] NOT NULL,
	[type] [int] NOT NULL,
	[baseVersion] [varchar](50) NOT NULL,
	[targetVersion] [varchar](50) NOT NULL,
	[implementationTime] [datetime] NOT NULL,
 CONSTRAINT [PK_PendingDDPUpdates] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[PendingDDPUpdates]  WITH CHECK ADD  CONSTRAINT [FK_PendingDDPUpdates_DDPUpdate] FOREIGN KEY([ddpUpdateId])
REFERENCES [dbo].[DDPUpdate] ([Id])
GO
ALTER TABLE [dbo].[PendingDDPUpdates] CHECK CONSTRAINT [FK_PendingDDPUpdates_DDPUpdate]