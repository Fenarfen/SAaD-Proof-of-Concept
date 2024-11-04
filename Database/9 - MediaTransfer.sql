USE [AML]
GO

/****** Object:  Table [dbo].[MediaTransfer]    Script Date: 04/11/2024 16:20:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MediaTransfer](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[MediaID] [int] NOT NULL,
	[OriginBranchID] [int] NOT NULL,
	[DestinationBranchID] [int] NOT NULL,
	[UserID] [int] NOT NULL,
	[Approved] [datetime] NULL,
	[Created] [datetime] NOT NULL,
	[Completed] [datetime] NULL,
 CONSTRAINT [PK_MediaTransfer] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[MediaTransfer] ADD  CONSTRAINT [DF_MediaTransfer_Created]  DEFAULT (getdate()) FOR [Created]
GO

ALTER TABLE [dbo].[MediaTransfer]  WITH CHECK ADD  CONSTRAINT [FK_MediaTransfer_Branch] FOREIGN KEY([OriginBranchID])
REFERENCES [dbo].[Branch] ([ID])
GO

ALTER TABLE [dbo].[MediaTransfer] CHECK CONSTRAINT [FK_MediaTransfer_Branch]
GO

ALTER TABLE [dbo].[MediaTransfer]  WITH CHECK ADD  CONSTRAINT [FK_MediaTransfer_Branch1] FOREIGN KEY([DestinationBranchID])
REFERENCES [dbo].[Branch] ([ID])
GO

ALTER TABLE [dbo].[MediaTransfer] CHECK CONSTRAINT [FK_MediaTransfer_Branch1]
GO

ALTER TABLE [dbo].[MediaTransfer]  WITH CHECK ADD  CONSTRAINT [FK_MediaTransfer_Media] FOREIGN KEY([MediaID])
REFERENCES [dbo].[Media] ([ID])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[MediaTransfer] CHECK CONSTRAINT [FK_MediaTransfer_Media]
GO

ALTER TABLE [dbo].[MediaTransfer]  WITH CHECK ADD  CONSTRAINT [FK_MediaTransfer_User] FOREIGN KEY([UserID])
REFERENCES [dbo].[User] ([ID])
GO

ALTER TABLE [dbo].[MediaTransfer] CHECK CONSTRAINT [FK_MediaTransfer_User]
GO


