using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Controllers;
using UserAPI.Interfaces;
using UserAPI.Models.Dtos;
using UserAPI.Models.Entities;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Text;
using Moq.Protected;

namespace UserAPI.Tests.Controllers;

[TestFixture]
public class AccountControllerTests
{
	private Mock<IDatabaseService> _mockDatabaseService;
	private Mock<HttpMessageHandler> _mockHttpHandler;
	private AccountController _controller;

	[SetUp]
	public void Setup()
	{
		_mockDatabaseService = new Mock<IDatabaseService>();

		_mockHttpHandler = new Mock<HttpMessageHandler>();
		var httpClient = new HttpClient(_mockHttpHandler.Object)
		{
			BaseAddress = new Uri("http://host.docker.internal:32784/api/")
		};

		_controller = new AccountController(_mockDatabaseService.Object);
		typeof(AccountController)
			.GetField("httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
			?.SetValue(_controller, httpClient);
	}

	[Test]
	public async Task CreateAccountAsync_WhenAccountCreatedSuccessfully_ReturnsOk()
	{
		var hashedPassword = AccountController.ComputeSHA256Hash("password123");
		var accountDto = new AccountCreateDto
		{
			Email = "test@example.com",
			Password = hashedPassword,
			FirstName = "John",
			LastName = "Doe"
		};

		_mockDatabaseService.Setup(db => db.DoesEmailExist(accountDto.Email)).Returns("false");
		_mockDatabaseService.Setup(db => db.CreateMemberUser(accountDto)).Returns("123");

		var httpResponse = new HttpResponseMessage(HttpStatusCode.OK);
		_mockHttpHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync", 
																	   ItExpr.IsAny<HttpRequestMessage>(),
																	   ItExpr.IsAny<CancellationToken>())
																			.ReturnsAsync(httpResponse);

		var result = await _controller.CreateAccountAsync(accountDto);

		Assert.That(result, Is.InstanceOf<OkObjectResult>());
		var okResult = result as OkObjectResult;
		Assert.That(okResult.Value, Is.Not.Null);
		Assert.That(okResult.Value.ToString(), Contains.Substring("Account created successfully"));
	}

	[Test]
	public async Task CreateAccountAsync_WhenEmailAlreadyExists_ReturnsConflict()
	{
		var accountDto = new AccountCreateDto
		{
			Email = "test@example.com",
			Password = "password123",
			FirstName = "John",
			LastName = "Doe"
		};

		_mockDatabaseService.Setup(db => db.DoesEmailExist(accountDto.Email)).Returns("true");

		var result = await _controller.CreateAccountAsync(accountDto);

		Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
		var conflictResult = result as ConflictObjectResult;
		Assert.That(conflictResult.Value.ToString(), Contains.Substring("Email is already registered"));
	}

	[Test]
	public void UpdateAccount_WhenAccountUpdatedSuccessfully_ReturnsOk()
	{
		var updatedAccount = new ProfileManagementDTO
		{
			ID = 1,
			Email = "updated@example.com",
			FirstName = "John",
			LastName = "Doe"
		};

		var existingAccount = new Account { ID = 1, Email = "old@example.com" };

		_mockDatabaseService.Setup(db => db.GetAccountByID(updatedAccount.ID)).Returns(existingAccount);
		_mockDatabaseService.Setup(db => db.DoesEmailExist(updatedAccount.Email)).Returns("false");
		_mockDatabaseService.Setup(db => db.EditAccount(updatedAccount)).Returns("success");

		var result = _controller.UpdateAccount(updatedAccount);

		Assert.That(result, Is.InstanceOf<OkObjectResult>());
		var okResult = result as OkObjectResult;
		Assert.That(okResult.Value.ToString(), Contains.Substring("Account updated successfully"));
	}

	[Test]
	public void UpdateAccount_WhenUserNotFound_ReturnsNotFound()
	{
		var updatedAccount = new ProfileManagementDTO
		{
			ID = 1,
			Email = "updated@example.com"
		};

		_mockDatabaseService.Setup(db => db.GetAccountByID(updatedAccount.ID)).Returns((Account)null);

		var result = _controller.UpdateAccount(updatedAccount);

		Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
		var notFoundResult = result as NotFoundObjectResult;
		Assert.That(notFoundResult.Value.ToString(), Contains.Substring("User not found"));
	}

	[Test]
	public void DoesEmailExist_WhenEmailIsValid_ReturnsOkWithResult()
	{
		string email = "test@example.com";
		_mockDatabaseService.Setup(db => db.DoesEmailExist(email)).Returns("true");

		var result = _controller.DoesEmailExist(email);

		Assert.That(result, Is.InstanceOf<OkObjectResult>());
		var okResult = result as OkObjectResult;
		Assert.That(okResult.Value.ToString(), Contains.Substring("exists = true"));
	}

	[Test]
	public void GetProfileManagementDTO_WhenTokenIsMissing_ReturnsBadRequest()
	{
		string token = "";

		var result = _controller.GetProfileManagementDTO(token);

		Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
		var badRequestResult = result as BadRequestObjectResult;
		Assert.That(badRequestResult.Value.ToString(), Contains.Substring("Token is missing"));
	}
}