using Moq;
using NUnit.Framework;
using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using AuthAPI.Models.Entities;
using AuthAPI.Services;
using System;
using AuthAPI.Models.DTOs;
using Assert = NUnit.Framework.Assert;

namespace AuthAPI.Tests;

[TestFixture]
public class DatabaseServiceTests
{
	private Mock<IDbConnection> _mockConnection;
	private Mock<IDbCommand> _mockCommand;
	private Mock<IDataReader> _mockReader;
	private string _connectionString = "TestConnectionString";
	private DatabaseService _databaseService;

	[SetUp]
	public void Setup()
	{
		_mockConnection = new Mock<IDbConnection>();
		_mockCommand = new Mock<IDbCommand>();
		_mockReader = new Mock<IDataReader>();

		var mockParameterCollection = new Mock<IDataParameterCollection>();
		_mockCommand.Setup(cmd => cmd.Parameters).Returns(mockParameterCollection.Object);

		var mockParameter = new Mock<IDbDataParameter>();
		_mockCommand.Setup(cmd => cmd.CreateParameter()).Returns(mockParameter.Object);

		_mockConnection.Setup(conn => conn.CreateCommand()).Returns(_mockCommand.Object);

		_databaseService = new DatabaseService(_mockConnection.Object);
	}

	[Test]
	public void StoreVerificationCode_ReturnsSuccess_WhenInsertSucceeds()
	{
		_mockCommand.Setup(cmd => cmd.ExecuteNonQuery()).Returns(1);

		var result = _databaseService.StoreVerificationCode(1, "123456");

		Assert.That(result, Is.EqualTo("success"));
	}

	[Test]
	public void StoreVerificationCode_ReturnsFailure_WhenInsertFails()
	{
		_mockCommand.Setup(cmd => cmd.ExecuteNonQuery()).Returns(0);

		var result = _databaseService.StoreVerificationCode(1, "123456");

		Assert.That(result, Is.EqualTo("failure"));
	}

	[Test]
	public void GetAccountByID_ReturnsAccount_WhenFound()
	{
		_mockReader.Setup(r => r.Read()).Returns(true);
		_mockReader.Setup(r => r["ID"]).Returns(1);
		_mockReader.Setup(r => r["TokenID"]).Returns(DBNull.Value); // simulating null TokenID
		_mockReader.Setup(r => r["RoleID"]).Returns(2);
		_mockReader.Setup(r => r["Password"]).Returns("password123");
		_mockReader.Setup(r => r["Email"]).Returns("test@test.com");
		_mockReader.Setup(r => r["FirstName"]).Returns("John");
		_mockReader.Setup(r => r["LastName"]).Returns("Doe");
		_mockReader.Setup(r => r["Created"]).Returns(DateTime.UtcNow);
		_mockReader.Setup(r => r["Verified"]).Returns(true);

		_mockCommand.Setup(cmd => cmd.ExecuteReader()).Returns(_mockReader.Object);

		var result = _databaseService.GetAccountByID(1);

		Assert.Multiple(() => 
		{
			Assert.That(result != null);
			Assert.That(result.ID, Is.EqualTo(1));
			Assert.That(result.TokenID, Is.EqualTo(null)); // simulating null TokenID
			Assert.That(result.RoleID, Is.EqualTo(2));
			Assert.That(result.Password, Is.EqualTo("password123"));
			Assert.That(result.Email, Is.EqualTo("test@test.com"));
			Assert.That(result.FirstName, Is.EqualTo("John"));
			Assert.That(result.LastName, Is.EqualTo("Doe"));
			Assert.That(result.Verified, Is.EqualTo(true));
		});
	}

	[Test]
	public void GetAccountByID_ReturnsNull_WhenNotFound()
	{
		_mockReader.Setup(r => r.Read()).Returns(false);
		_mockCommand.Setup(cmd => cmd.ExecuteReader()).Returns(_mockReader.Object);

		var result = _databaseService.GetAccountByID(1);

		Assert.That(result, Is.EqualTo(null));
	}

	[Test]
	public void CheckCode_ReturnsTrue_WhenCodeIsValid()
	{
		_mockCommand.Setup(cmd => cmd.ExecuteScalar()).Returns(1);

		var result = _databaseService.CheckCode("test@test.com", "123456");

		Assert.That(result, Is.EqualTo("true"));
	}

	[Test]
	public void CheckCode_ReturnsCodeExpired_WhenExpired()
	{
		_mockCommand.SetupSequence(cmd => cmd.ExecuteScalar())
					.Returns(0) // for valid code
					.Returns(1); // expired code

		var result = _databaseService.CheckCode("test@example.com", "123456");

		Assert.That(result, Is.EqualTo("Code has expired"));
	}

	[Test]
	public void CheckCode_ReturnsFalse_WhenCodeIsInvalid()
	{
		_mockCommand.SetupSequence(cmd => cmd.ExecuteScalar())
					.Returns(0) // valid code
					.Returns(0); // expired code

		var result = _databaseService.CheckCode("test@example.com", "123456");

		Assert.That(result, Is.EqualTo("false"));
	}
}
