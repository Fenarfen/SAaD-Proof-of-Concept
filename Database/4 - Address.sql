USE [AML]
GO

/****** Object:  Table [dbo].[Address]    Script Date: 04/11/2024 16:17:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Address](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CityID] [int] NOT NULL,
	[FirstLine] [varchar](max) NOT NULL,
	[SecondLine] [varchar](max) NOT NULL,
	[PostCode] [varchar](7) NOT NULL,
 CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Address] ADD  CONSTRAINT [DF_Address_SecondLine]  DEFAULT ('') FOR [SecondLine]
GO

ALTER TABLE [dbo].[Address]  WITH CHECK ADD  CONSTRAINT [FK_Address_City] FOREIGN KEY([CityID])
REFERENCES [dbo].[City] ([ID])
GO

ALTER TABLE [dbo].[Address] CHECK CONSTRAINT [FK_Address_City]
GO


