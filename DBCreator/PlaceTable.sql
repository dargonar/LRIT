USE [LRIT]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[Place_StandingOrder]') AND parent_object_id = OBJECT_ID(N'[dbo].[StandingOrder]'))
ALTER TABLE [dbo].[StandingOrder] DROP CONSTRAINT [Place_StandingOrder]
GO

USE [LRIT]
GO

/****** Object:  Table [dbo].[StandingOrder]    Script Date: 05/26/2010 14:59:27 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StandingOrder]') AND type in (N'U'))
DROP TABLE [dbo].[StandingOrder]
GO

/*PLACE*/
USE [LRIT]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[ContractingGoverment_Place]') AND parent_object_id = OBJECT_ID(N'[dbo].[Place]'))
ALTER TABLE [dbo].[Place] DROP CONSTRAINT [ContractingGoverment_Place]
GO

USE [LRIT]
GO

/****** Object:  Table [dbo].[Place]    Script Date: 05/26/2010 14:59:46 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Place]') AND type in (N'U'))
DROP TABLE [dbo].[Place]
GO

USE [LRIT]
GO

/****** Object:  Table [dbo].[Place]    Script Date: 05/26/2010 14:59:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Place](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ContractingGovermentId] [int] NOT NULL,
	[PlaceStringId] [varchar](32) NOT NULL,
	[Name] [varchar](max) NOT NULL,
	[AreaType] [varchar](64) NOT NULL,
	[Area] [varbinary](max) NULL,
	[IsTerritory] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Place] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Place]  WITH CHECK ADD  CONSTRAINT [ContractingGoverment_Place] FOREIGN KEY([ContractingGovermentId])
REFERENCES [dbo].[ContractingGoverment] ([Id])
GO

ALTER TABLE [dbo].[Place] CHECK CONSTRAINT [ContractingGoverment_Place]
GO

/*PLACE END*/



















USE [LRIT]
GO

/****** Object:  Table [dbo].[StandingOrder]    Script Date: 05/26/2010 14:59:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[StandingOrder](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlaceId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.StandingOrder] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[StandingOrder]  WITH CHECK ADD  CONSTRAINT [Place_StandingOrder] FOREIGN KEY([PlaceId])
REFERENCES [dbo].[Place] ([Id])
GO

ALTER TABLE [dbo].[StandingOrder] CHECK CONSTRAINT [Place_StandingOrder]
GO


