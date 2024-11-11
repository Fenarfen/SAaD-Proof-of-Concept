USE [master]
GO

/****** Object:  Database [AML]    Script Date: 11/11/2024 00:28:13 ******/
CREATE DATABASE [AML]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'AML', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER01\MSSQL\DATA\AML.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'AML_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER01\MSSQL\DATA\AML_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
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

ALTER DATABASE [AML] SET  READ_WRITE 
GO

