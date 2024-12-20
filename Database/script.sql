USE [master]
GO

drop database [AML]

/****** Object:  Database [AML]    Script Date: 14/11/2024 15:16:24 ******/
CREATE DATABASE [AML]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'AML', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\AML.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'AML_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\DATA\AML_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [AML] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [AML].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [AML] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [AML] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [AML] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [AML] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [AML] SET ARITHABORT OFF 
GO
ALTER DATABASE [AML] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [AML] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [AML] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [AML] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [AML] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [AML] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [AML] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [AML] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [AML] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [AML] SET  DISABLE_BROKER 
GO
ALTER DATABASE [AML] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [AML] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [AML] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [AML] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [AML] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [AML] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [AML] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [AML] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [AML] SET  MULTI_USER 
GO
ALTER DATABASE [AML] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [AML] SET DB_CHAINING OFF 
GO
ALTER DATABASE [AML] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [AML] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [AML] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [AML] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [AML] SET QUERY_STORE = OFF
GO
USE [AML]
GO
/****** Object:  User [inventoryAPI]    Script Date: 14/11/2024 15:16:24 ******/
CREATE USER [inventoryAPI] FOR LOGIN [inventoryAPI] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [authAPI]    Script Date: 14/11/2024 15:16:24 ******/
CREATE USER [authAPI] FOR LOGIN [authAPI] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_datareader] ADD MEMBER [inventoryAPI]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [inventoryAPI]
GO
ALTER ROLE [db_datareader] ADD MEMBER [authAPI]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [authAPI]
GO
ALTER ROLE [db_datareader] ADD MEMBER [userAPI]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [userAPI]
GO
/****** Object:  Table [dbo].[Account]    Script Date: 14/11/2024 15:16:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Account](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TokenID] [int] NULL,
	[RoleID] [int] NOT NULL,
	[Email] [varchar](max) NOT NULL,
	[Password] [varchar](max) NOT NULL,
	[FirstName] [varchar](max) NOT NULL,
	[LastName] [varchar](max) NOT NULL,
	[Created] [datetime] NOT NULL,
	[Verified] [bit] NOT NULL,
 CONSTRAINT [PK_Account] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserVerificationCode]    Script Date: 26/11/2024 21:50:23 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AccountVerificationCode](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AccountID] [int] NOT NULL,
	[Code] [varchar](10) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Branch]    Script Date: 14/11/2024 15:16:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Branch](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](max) NOT NULL,
	[AddressFirstLine] [varchar](max) NOT NULL,
	[AddressSecondLine] [varchar](max) NOT NULL,
	[City] [varchar](50) NOT NULL,
	[PostCode] [varchar](7) NOT NULL,
	[Opened] [datetime] NOT NULL,
 CONSTRAINT [PK_Branch] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Media]    Script Date: 14/11/2024 15:16:25 ******/
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
	[Genre] [varchar](max) NOT NULL,
	[Type] [varchar](max) NOT NULL,
 CONSTRAINT [PK_Media] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MediaLoan]    Script Date: 14/11/2024 15:16:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MediaLoan](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[MediaID] [int] NOT NULL,
	[AccountID] [int] NOT NULL,
	[BranchID] [int] NOT NULL,
	[LoanedDate] [datetime] NOT NULL,
	[DueDate] [datetime] NOT NULL,
	[ReturnedDate] [datetime] NULL,
	[Status] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MediaTransfer]    Script Date: 14/11/2024 15:16:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MediaTransfer](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[MediaID] [int] NOT NULL,
	[OriginBranchID] [int] NOT NULL,
	[DestinationBranchID] [int] NOT NULL,
	[AccountID] [int] NOT NULL,
	[Approved] [datetime] NULL,
	[Created] [datetime] NOT NULL,
	[Completed] [datetime] NULL,
 CONSTRAINT [PK_MediaTransfer] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MemberAddress]    Script Date: 14/11/2024 15:16:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Address](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL, 
	[FirstLine] [varchar](100) NOT NULL,
	[SecondLine] [varchar](100) NULL,
	[ThirdLine] [varchar](100) NULL,
	[FourthLine] [varchar](100) NULL,
	[City] [varchar](50) NOT NULL,
	[County] [varchar](50) NULL,
	[Country] [varchar](50) NULL,
	[Postcode] [varchar](20) NOT NULL,
	[IsDefault] [bit] NOT NULL
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 14/11/2024 15:16:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](max) NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Token]    Script Date: 14/11/2024 15:16:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Token](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[Value] [varchar](max) NOT NULL,
	[Created] [datetime] NOT NULL,
 CONSTRAINT [PK_Token] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Account] ON 

INSERT [dbo].[Account] ([ID], [TokenID], [RoleID], [Email], [Password], [FirstName], [LastName], [Created], [Verified]) VALUES (1, 1, 2, N'sam@sam.co.uk', N'Pass', N'Sam', N'Sam', CAST(N'2024-11-04T18:09:57.527' AS DateTime), 1)
INSERT [dbo].[Account] ([ID], [TokenID], [RoleID], [Email], [Password], [FirstName], [LastName], [Created], [Verified]) VALUES (4, 6, 2, N'jeff@jeff.co.uk', N'Pass', N'Jeff', N'Jeff', CAST(N'2024-11-01T18:12:00.000' AS DateTime), 1)
INSERT [dbo].[Account] ([ID], [TokenID], [RoleID], [Email], [Password], [FirstName], [LastName], [Created], [Verified]) VALUES (5, 7, 2, N'bob@bob.co.uk', N'Pass', N'Bob', N'Bob', CAST(N'2024-11-04T18:12:49.237' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[Account] OFF
GO
SET IDENTITY_INSERT [dbo].[Branch] ON 

INSERT [dbo].[Branch] ([ID], [Name], [AddressFirstLine], [AddressSecondLine], [City], [PostCode], [Opened]) VALUES (1, N'Hallam Library', N'121 Arundel Street', N'', N'Sheffield', N'S12NT', CAST(N'2024-10-10T00:00:00.000' AS DateTime))
INSERT [dbo].[Branch] ([ID], [Name], [AddressFirstLine], [AddressSecondLine], [City], [PostCode], [Opened]) VALUES (3, N'Devonshire Park Library', N'112 Devonshire Street', N'', N'Sheffield', N'S37SF', CAST(N'2024-09-09T00:00:00.000' AS DateTime))
INSERT [dbo].[Branch] ([ID], [Name], [AddressFirstLine], [AddressSecondLine], [City], [PostCode], [Opened]) VALUES (4, N'The Savoy Centre', N'9 Savoy Way', N'', N'London', N'WC2R0BP', CAST(N'2024-11-01T00:00:00.000' AS DateTime))
SET IDENTITY_INSERT [dbo].[Branch] OFF
GO
SET IDENTITY_INSERT [dbo].[Media] ON 

INSERT [dbo].[Media] ([ID], [BranchID], [Title], [Released], [Author], [Genre], [Type]) VALUES (2, 1, N'Catching Fire (Hunger Games, Book Two)', CAST(N'2009-09-01T00:00:00.000' AS DateTime), N'Suzanne Collins', N'Scifi', N'book')
INSERT [dbo].[Media] ([ID], [BranchID], [Title], [Released], [Author], [Genre], [Type]) VALUES (3, 1, N'The Hunger Games: Catching Fire', CAST(N'2013-11-21T00:00:00.000' AS DateTime), N'Francis Lawrence', N'Scifi', N'film')
INSERT [dbo].[Media] ([ID], [BranchID], [Title], [Released], [Author], [Genre], [Type]) VALUES (5, 3, N'Staying on', CAST(N'1977-01-01T00:00:00.000' AS DateTime), N'Paul Scott', N'Historical Fiction', N'book')
INSERT [dbo].[Media] ([ID], [BranchID], [Title], [Released], [Author], [Genre], [Type]) VALUES (6, 3, N'Jesus Calling: 365 Devotions for Kids', CAST(N'2010-10-10T00:00:00.000' AS DateTime), N'Sarah Young', N'Religious', N'book')
INSERT [dbo].[Media] ([ID], [BranchID], [Title], [Released], [Author], [Genre], [Type]) VALUES (7, 3, N'American Prometheus', CAST(N'2005-04-05T00:00:00.000' AS DateTime), N'Kai Bird', N'Biography', N'book')
INSERT [dbo].[Media] ([ID], [BranchID], [Title], [Released], [Author], [Genre], [Type]) VALUES (8, 4, N'Pout-Pout Fish', CAST(N'2008-01-01T00:00:00.000' AS DateTime), N'Dan Hanna', N'Fiction', N'book')
INSERT [dbo].[Media] ([ID], [BranchID], [Title], [Released], [Author], [Genre], [Type]) VALUES (9, 4, N'Goth Girl and the Ghost of a Mouse', CAST(N'2013-10-31T00:00:00.000' AS DateTime), N'Chris Riddell', N'Fiction', N'book')
INSERT [dbo].[Media] ([ID], [BranchID], [Title], [Released], [Author], [Genre], [Type]) VALUES (10, 1, N'Independance Day: Resurgence', CAST(N'2016-06-23T00:00:00.000' AS DateTime), N'Roland Emmerich', N'Scifi', N'film')
INSERT [dbo].[Media] ([ID], [BranchID], [Title], [Released], [Author], [Genre], [Type]) VALUES (12, 3, N'Forrest Gump', CAST(N'1994-10-07T00:00:00.000' AS DateTime), N'Robert Zemeckis', N'Comedy', N'film')
INSERT [dbo].[Media] ([ID], [BranchID], [Title], [Released], [Author], [Genre], [Type]) VALUES (13, 4, N'Gladiator', CAST(N'2000-05-12T00:00:00.000' AS DateTime), N'Riddly Scott', N'Action', N'film')
INSERT [dbo].[Media] ([ID], [BranchID], [Title], [Released], [Author], [Genre], [Type]) VALUES (14, 1, N'Need for Speed: ProStreet', CAST(N'2007-10-31T00:00:00.000' AS DateTime), N'Electronic Arts', N'Racing', N'game')
INSERT [dbo].[Media] ([ID], [BranchID], [Title], [Released], [Author], [Genre], [Type]) VALUES (15, 3, N'Half-Life 2', CAST(N'2004-11-16T00:00:00.000' AS DateTime), N'Valve', N'Scifi', N'game')
INSERT [dbo].[Media] ([ID], [BranchID], [Title], [Released], [Author], [Genre], [Type]) VALUES (16, 4, N'Roller Tycoon 3', CAST(N'2004-11-02T00:00:00.000' AS DateTime), N'Frontier Developments', N'Simulation', N'game')
SET IDENTITY_INSERT [dbo].[Media] OFF
GO
SET IDENTITY_INSERT [dbo].[MediaLoan] ON 

INSERT [dbo].[MediaLoan] ([ID], [MediaID], [AccountID], [BranchID], [LoanedDate], [DueDate], [ReturnedDate], [Status]) VALUES (10, 2, 1, 1, CAST(N'2024-11-02T16:56:34.637' AS DateTime), CAST(N'2024-11-17T16:56:34.637' AS DateTime), NULL, N'Active')
INSERT [dbo].[MediaLoan] ([ID], [MediaID], [AccountID], [BranchID], [LoanedDate], [DueDate], [ReturnedDate], [Status]) VALUES (11, 3, 4, 1, CAST(N'2024-10-28T16:56:34.637' AS DateTime), CAST(N'2024-11-14T16:56:34.637' AS DateTime), NULL, N'Active')
INSERT [dbo].[MediaLoan] ([ID], [MediaID], [AccountID], [BranchID], [LoanedDate], [DueDate], [ReturnedDate], [Status]) VALUES (12, 5, 5, 3, CAST(N'2024-10-13T16:56:34.637' AS DateTime), CAST(N'2024-11-02T16:56:34.637' AS DateTime), CAST(N'2024-11-14T16:56:34.637' AS DateTime), N'Returned')
INSERT [dbo].[MediaLoan] ([ID], [MediaID], [AccountID], [BranchID], [LoanedDate], [DueDate], [ReturnedDate], [Status]) VALUES (16, 6, 1, 1, CAST(N'2024-10-18T19:16:50.423' AS DateTime), CAST(N'2024-10-13T19:16:50.423' AS DateTime), NULL, N'Overdue')
SET IDENTITY_INSERT [dbo].[MediaLoan] OFF
GO
SET IDENTITY_INSERT [dbo].[MediaTransfer] ON 

INSERT [dbo].[MediaTransfer] ([ID], [MediaID], [OriginBranchID], [DestinationBranchID], [AccountID], [Approved], [Created], [Completed]) VALUES (1, 5, 3, 1, 1, NULL, CAST(N'2024-11-06T15:23:28.160' AS DateTime), NULL)
INSERT [dbo].[MediaTransfer] ([ID], [MediaID], [OriginBranchID], [DestinationBranchID], [AccountID], [Approved], [Created], [Completed]) VALUES (2, 3, 1, 3, 1, NULL, CAST(N'2024-11-07T12:14:36.163' AS DateTime), NULL)
SET IDENTITY_INSERT [dbo].[MediaTransfer] OFF
GO
SET IDENTITY_INSERT [dbo].[MemberAddress] ON 

INSERT [dbo].[MemberAddress] ([ID], [FirstLine], [SecondLine], [ThirdLine], [FourthLine], [City], [County], [Country], [Postcode]) VALUES (2, N'line1', N'line2', N'line3', N'line4', N'city', N'county', N'country', N'postcode')
INSERT [dbo].[MemberAddress] ([ID], [FirstLine], [SecondLine], [ThirdLine], [FourthLine], [City], [County], [Country], [Postcode]) VALUES (3, N'y', N'u', N'i', N'o', N'a', N's', N'd', N'f')
SET IDENTITY_INSERT [dbo].[MemberAddress] OFF
GO
SET IDENTITY_INSERT [dbo].[Role] ON 

INSERT [dbo].[Role] ([ID], [Name]) VALUES (1, N'Member')
INSERT [dbo].[Role] ([ID], [Name]) VALUES (2, N'Manager')
INSERT [dbo].[Role] ([ID], [Name]) VALUES (3, N'Librarian')
INSERT [dbo].[Role] ([ID], [Name]) VALUES (4, N'Admin')
SET IDENTITY_INSERT [dbo].[Role] OFF
GO
SET IDENTITY_INSERT [dbo].[Token] ON 

INSERT [dbo].[Token] ([ID], [UserID], [Value], [Created]) VALUES (1, 1, N'111111111111111', CAST(N'2024-11-04T18:13:26.890' AS DateTime))
INSERT [dbo].[Token] ([ID], [UserID], [Value], [Created]) VALUES (6, 4, N'222222222222222', CAST(N'2024-11-04T18:14:13.940' AS DateTime))
INSERT [dbo].[Token] ([ID], [UserID], [Value], [Created]) VALUES (7, 5, N'555555555555555', CAST(N'2024-11-04T18:14:20.340' AS DateTime))
SET IDENTITY_INSERT [dbo].[Token] OFF
GO
ALTER TABLE [dbo].[Branch] ADD  CONSTRAINT [DF_Branch_Opened]  DEFAULT (getdate()) FOR [Opened]
GO
ALTER TABLE [dbo].[MediaTransfer] ADD  CONSTRAINT [DF_MediaTransfer_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[Token] ADD  CONSTRAINT [DF_Token_Created]  DEFAULT (getdate()) FOR [Created]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [FK_Account_Role] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Role] ([ID])
GO
ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [FK_Account_Role]
GO
ALTER TABLE [dbo].[Account]  WITH CHECK ADD  CONSTRAINT [FK_Account_Token] FOREIGN KEY([TokenID])
REFERENCES [dbo].[Token] ([ID])
GO
ALTER TABLE [dbo].[Account] CHECK CONSTRAINT [FK_Account_Token]
GO
ALTER TABLE [dbo].[Media]  WITH CHECK ADD  CONSTRAINT [FK_Media_Branch] FOREIGN KEY([BranchID])
REFERENCES [dbo].[Branch] ([ID])
GO
ALTER TABLE [dbo].[Media] CHECK CONSTRAINT [FK_Media_Branch]
GO
ALTER TABLE [dbo].[MediaLoan]  WITH CHECK ADD FOREIGN KEY([AccountID])
REFERENCES [dbo].[Account] ([ID])
GO
ALTER TABLE [dbo].[MediaLoan]  WITH CHECK ADD FOREIGN KEY([BranchID])
REFERENCES [dbo].[Branch] ([ID])
GO
ALTER TABLE [dbo].[MediaLoan]  WITH CHECK ADD FOREIGN KEY([MediaID])
REFERENCES [dbo].[Media] ([ID])
GO
ALTER TABLE [dbo].[MediaTransfer]  WITH CHECK ADD  CONSTRAINT [FK_MediaTransfer_Account] FOREIGN KEY([AccountID])
REFERENCES [dbo].[Account] ([ID])
GO
ALTER TABLE [dbo].[MediaTransfer] CHECK CONSTRAINT [FK_MediaTransfer_Account]
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
GO
ALTER TABLE [dbo].[MediaTransfer] CHECK CONSTRAINT [FK_MediaTransfer_Media]
GO
ALTER TABLE [dbo].[MediaLoan]  WITH CHECK ADD CHECK  (([Status]='Overdue' OR [Status]='Returned' OR [Status]='Active'))
GO
USE [master]
GO
ALTER DATABASE [AML] SET  READ_WRITE 
GO
