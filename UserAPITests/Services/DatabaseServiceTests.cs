using Moq;
using NUnit.Framework;
using System.Data;
using UserAPI.Models.Dtos;
using UserAPI.Models.Entities;
using UserAPI.Services;

namespace UserAPI.Tests.Services;

[TestFixture]
public class DatabaseServiceTests
{
	private Mock<IDbConnection> _mockConnection;
	private Mock<IDbCommand> _mockCommand;
	private Mock<IDataReader> _mockDataReader;
	private DatabaseService _service;

	[SetUp]
	public void Setup()
	{
		_mockConnection = new Mock<IDbConnection>();
		_mockCommand = new Mock<IDbCommand>();
		_mockDataReader = new Mock<IDataReader>();
		var mockParameter = new Mock<IDbDataParameter>();

		_mockCommand.Setup(cmd => cmd.CreateParameter()).Returns(mockParameter.Object);
		_mockCommand.Setup(cmd => cmd.Parameters).Returns(new Mock<IDataParameterCollection>().Object);
		_mockConnection.Setup(conn => conn.CreateCommand()).Returns(_mockCommand.Object);

		_service = new DatabaseService(_mockConnection.Object);
	}


	[Test]
	public void CreateMemberUser_WhenCalled_ReturnsNewUserId()
	{
		var dto = new AccountCreateDto
		{
			Email = "test@example.com",
			FirstName = "John",
			LastName = "Doe",
			Password = "password123"
		};

		_mockCommand.Setup(cmd => cmd.ExecuteScalar()).Returns(1);

		var result = _service.CreateMemberUser(dto);

		Assert.That(result, Is.EqualTo("1"));
	}

	[Test]
	public void EditAccount_WhenUpdateSuccessful_ReturnsSuccess()
	{
		var dto = new ProfileManagementDTO
		{
			ID = 1,
			Email = "john@example.com",
			FirstName = "John",
			LastName = "Doe",
			Addresses = new List<Address>()
			{
				new Address
				{
					ID = 1,
					AccountID = 1,
					FirstLine = "123 Main Street",
					City = "Test City",
					PostCode = "12345",
					IsDefault = true
				}
			}
		};

		var mockReader = new Mock<IDataReader>();

		mockReader.SetupSequence(reader => reader.Read())
				  .Returns(true)    // A row exists
				  .Returns(false);  // End of data

		mockReader.Setup(reader => reader.GetInt32(0)).Returns(1);                  // ID
		mockReader.Setup(reader => reader.GetInt32(1)).Returns(1);                  // AccountID
		mockReader.Setup(reader => reader.GetString(2)).Returns("123 Main Street"); // FirstLine
		mockReader.Setup(reader => reader.IsDBNull(3)).Returns(true);               // SecondLine is null
		mockReader.Setup(reader => reader.IsDBNull(4)).Returns(true);               // ThirdLine is null
		mockReader.Setup(reader => reader.IsDBNull(5)).Returns(true);               // FourthLine is null
		mockReader.Setup(reader => reader.GetString(6)).Returns("Test City");       // City
		mockReader.Setup(reader => reader.IsDBNull(7)).Returns(true);               // County is null
		mockReader.Setup(reader => reader.IsDBNull(8)).Returns(true);               // Country is null
		mockReader.Setup(reader => reader.GetString(9)).Returns("12345");           // PostCode
		mockReader.Setup(reader => reader.GetBoolean(10)).Returns(true);            // IsDefault

		_mockCommand.Setup(cmd => cmd.ExecuteNonQuery()).Returns(1);
		_mockCommand.Setup(cmd => cmd.ExecuteReader()).Returns(mockReader.Object);

		_mockConnection.Setup(conn => conn.CreateCommand()).Returns(_mockCommand.Object);

		var result = _service.EditAccount(dto);

		Assert.That(result, Is.EqualTo("success"));
	}

	[Test]
	public void DeleteAddress_WhenNoAddressFound_ReturnsNotFound()
	{
		int addressId = 123;
		_mockCommand.Setup(cmd => cmd.ExecuteNonQuery()).Returns(0);

		var result = _service.DeleteAddress(addressId);

		Assert.That(result, Is.EqualTo("not found"));
	}

	[Test]
	public void DeleteAddress_WhenAddressIsDeleted_ReturnsSuccess()
	{
		int addressId = 123;
		_mockCommand.Setup(cmd => cmd.ExecuteNonQuery()).Returns(1);

		var result = _service.DeleteAddress(addressId);

		Assert.That(result, Is.EqualTo("success"));
	}

	[Test]
	public void GetAccountByID_WhenAccountExists_ReturnsAccount()
	{
		var accountID = 1;
		var tokenID = 1;
		var roleID = 1;
		var password = "hashedpassword";
		var email = "test@example.com";
		var firstName = "John";
		var lastName = "Doe";
		var created = DateTime.Now;
		var verified = true;

		var mockReader = new Mock<IDataReader>();
		mockReader.SetupSequence(reader => reader.Read())
				  .Returns(true)	// First row
				  .Returns(false);	// End of data

		mockReader.Setup(reader => reader["ID"]).Returns(accountID);
		mockReader.Setup(reader => reader["TokenID"]).Returns(tokenID);
		mockReader.Setup(reader => reader["RoleID"]).Returns(roleID);
		mockReader.Setup(reader => reader["Password"]).Returns(password);
		mockReader.Setup(reader => reader["Email"]).Returns(email);
		mockReader.Setup(reader => reader["FirstName"]).Returns(firstName);
		mockReader.Setup(reader => reader["LastName"]).Returns(lastName);
		mockReader.Setup(reader => reader["Created"]).Returns(created);
		mockReader.Setup(reader => reader["Verified"]).Returns(verified);

		var mockParameter = new Mock<IDbDataParameter>();
		var mockCommand = new Mock<IDbCommand>();
		mockCommand.Setup(cmd => cmd.ExecuteReader()).Returns(mockReader.Object);
		mockCommand.Setup(cmd => cmd.CreateParameter()).Returns(mockParameter.Object);
		mockCommand.Setup(cmd => cmd.Parameters).Returns(new Mock<IDataParameterCollection>().Object);
		var mockConnection = new Mock<IDbConnection>();
		mockConnection.Setup(conn => conn.CreateCommand()).Returns(mockCommand.Object);
		var service = new DatabaseService(mockConnection.Object);

		var result = service.GetAccountByID(accountID);

		Assert.Multiple(() =>
		{
			Assert.That(result, Is.Not.Null);
			Assert.That(result.ID, Is.EqualTo(accountID));
			Assert.That(result.TokenID, Is.EqualTo(tokenID));
			Assert.That(result.RoleID, Is.EqualTo(roleID));
			Assert.That(result.Password, Is.EqualTo(password));
			Assert.That(result.Email, Is.EqualTo(email));
			Assert.That(result.FirstName, Is.EqualTo(firstName));
			Assert.That(result.LastName, Is.EqualTo(lastName));
			Assert.That(result.Created, Is.EqualTo(created));
			Assert.That(result.Verified, Is.EqualTo(verified));
		});
	}

	[Test]
	public void DoesEmailExist_WhenEmailFound_ReturnsTrue()
	{
		string email = "test@example.com";
		_mockCommand.Setup(cmd => cmd.ExecuteScalar()).Returns(1);

		var result = _service.DoesEmailExist(email);

		Assert.That(result, Is.EqualTo("true"));
	}

	[Test]
	public void DoesEmailExist_WhenEmailNotFound_ReturnsFalse()
	{
		string email = "test@example.com";
		_mockCommand.Setup(cmd => cmd.ExecuteScalar()).Returns(0);

		var result = _service.DoesEmailExist(email);

		Assert.That(result, Is.EqualTo("false"));
	}
}
