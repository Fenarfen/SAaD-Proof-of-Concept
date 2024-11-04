USE [AML]
GO

/****** Object:  Table [dbo].[Branch]    Script Date: 04/11/2024 16:19:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Branch](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AddressID] [int] NOT NULL,
	[Name] [varchar](max) NOT NULL,
	[Opened] [datetime] NOT NULL,
 CONSTRAINT [PK_Branch] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Branch] ADD  CONSTRAINT [DF_Branch_Opened]  DEFAULT (getdate()) FOR [Opened]
GO

ALTER TABLE [dbo].[Branch]  WITH CHECK ADD  CONSTRAINT [FK_Branch_Address] FOREIGN KEY([AddressID])
REFERENCES [dbo].[Address] ([ID])
GO

ALTER TABLE [dbo].[Branch] CHECK CONSTRAINT [FK_Branch_Address]
GO


