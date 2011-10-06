USE [LRIT]
GO

/****** Object:  Table [dbo].[PriceUserProvider]    Script Date: 06/09/2011 20:41:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[PriceUserProvider](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[priceId] [int] NOT NULL,
	[dataProviderID] [varchar](8) NOT NULL,
 CONSTRAINT [PK_PricesUserProviders] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[PriceUserProvider]  WITH CHECK ADD  CONSTRAINT [FK_PricesUserProvider_Prices] FOREIGN KEY([priceId])
REFERENCES [dbo].[Price] ([id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[PriceUserProvider] CHECK CONSTRAINT [FK_PricesUserProvider_Prices]
GO


