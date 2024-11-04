USE [AML]
GO

/****** Object:  Table [dbo].[Media]    Script Date: 04/11/2024 16:19:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Media](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[BranchID] [int] NOT NULL,
	[Title] [varchar](max) NOT NULL,
	[Released] [datetime] NOT NULL,
	[Author] [varchar](max) NOT NULL,
	[Type] [varchar](max) NOT NULL,
 CONSTRAINT [PK_Media] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Media]  WITH CHECK ADD  CONSTRAINT [FK_Media_Branch] FOREIGN KEY([BranchID])
REFERENCES [dbo].[Branch] ([ID])
GO

ALTER TABLE [dbo].[Media] CHECK CONSTRAINT [FK_Media_Branch]
GO


