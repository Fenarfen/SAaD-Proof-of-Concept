using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Configuration;
using AuthAPI.Controllers;
using AuthAPI.Interfaces;
using AuthAPI.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Assert = NUnit.Framework.Assert;
using AuthAPI.Models.Requests;
using AuthAPI.Models.DTOs;
using System.Text.RegularExpressions;

[TestFixture]
public class AuthControllerTests
{
	private Mock<IConfiguration> _mockConfig;
	private Mock<IDatabaseService> _mockDatabaseService;
	private Mock<IEmailService> _mockEmailService;
	private AuthController _controller;
	private const string ValidInventoryApiKey = "testinventorykey";
	private const string ValidAuthorizationHeader = "Bearer testinventorykey";

	[SetUp]
	public void SetUp()
	{
		_mockConfig = new Mock<IConfiguration>();
		_mockDatabaseService = new Mock<IDatabaseService>();
		_mockEmailService = new Mock<IEmailService>();
		_controller = new AuthController(_mockConfig.Object, _mockDatabaseService.Object, _mockEmailService.Object);
	}

	[Test]
	public void SendEmailVerification_AuthorizationHeaderMissing_ReturnsUnauthorized()
	{
		var result = _controller.SendEmailVerification(1, null);

		Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
		var unauthorizedResult = result as UnauthorizedObjectResult;
		Assert.That(
			unauthorizedResult?.Value?.GetType().GetProperty("message")?.GetValue(unauthorizedResult.Value, null),
			Is.EqualTo("Authorization header is missing or invalid.")
		);
	}

	[Test]
	public void SendEmailVerification_InvalidUserKey_ReturnsUnauthorized()
	{
		var result = _controller.SendEmailVerification(1, "Bearer invalidkey");

		Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
		var unauthorizedResult = result as UnauthorizedObjectResult;

		Assert.That(
			unauthorizedResult?.Value?.GetType().GetProperty("message")?.GetValue(unauthorizedResult.Value, null),
			Is.EqualTo("Invalid User Key.")
		);
	}

	[Test]
	public void SendEmailVerification_UserNotFound_ReturnsBadRequest()
	{
		_mockDatabaseService.Setup(db => db.GetAccountByID(It.IsAny<int>())).Returns((Account)null);

		var result = _controller.SendEmailVerification(1, "Bearer testuserkey");

		Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
		var badRequestResult = result as BadRequestObjectResult;

		Assert.That(
			badRequestResult?.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value, null),
			Is.EqualTo("User does not exist.")
		);
	}

	[Test]
	public void SendEmailVerification_AccountAlreadyVerified_ReturnsBadRequest()
	{
		var mockAccount = new Account { ID = 1, Verified = true };
		_mockDatabaseService.Setup(db => db.GetAccountByID(It.IsAny<int>())).Returns(mockAccount);

		var result = _controller.SendEmailVerification(1, "Bearer testuserkey");

		Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
		var badRequestResult = result as BadRequestObjectResult;

		Assert.That(
			badRequestResult?.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value, null),
			Is.EqualTo("Account is already verified")
		);
	}

	[Test]
	public void SendEmailVerification_DatabaseFailsToStoreCode_ReturnsInternalServerError()
	{
		var mockAccount = new Account { ID = 1, Verified = false };
		_mockDatabaseService.Setup(db => db.GetAccountByID(It.IsAny<int>())).Returns(mockAccount);
		_mockDatabaseService.Setup(db => db.StoreVerificationCode(It.IsAny<int>(), It.IsAny<string>())).Returns("failure");

		var result = _controller.SendEmailVerification(1, "Bearer testuserkey");

		Assert.That(result, Is.InstanceOf<ObjectResult>());
		var serverErrorResult = result as ObjectResult;
		Assert.That(
			serverErrorResult?.StatusCode,
			Is.EqualTo(500)
		);
		Assert.That((bool)(serverErrorResult?.Value?.ToString().Contains("Failed when writing code to database")));
	}

	[Test]
	public void SendEmailVerification_EmailSendingFails_ReturnsInternalServerError()
	{
		var mockAccount = new Account { ID = 1, Verified = false, Email = "test@example.com" };
		_mockDatabaseService.Setup(db => db.GetAccountByID(It.IsAny<int>())).Returns(mockAccount);
		_mockDatabaseService.Setup(db => db.StoreVerificationCode(It.IsAny<int>(), It.IsAny<string>())).Returns("success");
		_mockEmailService.Setup(email => email.SendVerificationEmail(It.IsAny<string>(), It.IsAny<string>())).Returns("failure");

		var result = _controller.SendEmailVerification(1, "Bearer testuserkey");

		Assert.That(result, Is.InstanceOf<ObjectResult>());
		var serverErrorResult = result as ObjectResult;
		Assert.That(
			serverErrorResult?.StatusCode,
			Is.EqualTo(500)
		);
		Assert.That((bool)serverErrorResult?.Value?.ToString().Contains("Failed when sending Email"));
	}

	[Test]
	public void SendEmailVerification_SuccessfulExecution_ReturnsOk()
	{
		var mockAccount = new Account { ID = 1, Verified = false, Email = "test@example.com" };
		_mockDatabaseService.Setup(db => db.GetAccountByID(It.IsAny<int>())).Returns(mockAccount);
		_mockDatabaseService.Setup(db => db.StoreVerificationCode(It.IsAny<int>(), It.IsAny<string>())).Returns("success");
		_mockEmailService.Setup(email => email.SendVerificationEmail(It.IsAny<string>(), It.IsAny<string>())).Returns("success");

		var result = _controller.SendEmailVerification(1, "Bearer testuserkey");

		Assert.That(result, Is.InstanceOf<OkObjectResult>());
		var okResult = result as OkObjectResult;
		Assert.That(
			okResult?.Value?.GetType().GetProperty("message")?.GetValue(okResult.Value, null),
			Is.EqualTo("success")
		);
	}

	[Test]
	public void VerifyAccount_AccountNotFound_ReturnsBadRequest()
	{
		_mockDatabaseService.Setup(db => db.GetAccountByEmail(It.IsAny<string>())).Returns((Account)null);
		var request = new VerifyCodeRequest { Email = "test@example.com", Code = "123456" };

		var result = _controller.VerifyAccount(request);

		Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
		var badRequestResult = result as BadRequestObjectResult;
		Assert.That(
			badRequestResult?.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value, null),
			Is.EqualTo("User does not exist.")
		);
	}

	[Test]
	public void VerifyAccount_IncorrectCode_ReturnsBadRequest()
	{
		var mockAccount = new Account { Email = "test@example.com" };
		_mockDatabaseService.Setup(db => db.GetAccountByEmail(It.IsAny<string>())).Returns(mockAccount);
		_mockDatabaseService.Setup(db => db.CheckCode(It.IsAny<string>(), It.IsAny<string>())).Returns("false");
		var request = new VerifyCodeRequest { Email = "test@example.com", Code = "wrongcode" };

		var result = _controller.VerifyAccount(request);

		Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
		var badRequestResult = result as BadRequestObjectResult;
		Assert.That(
			badRequestResult?.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value, null),
			Is.EqualTo("Code is incorrect.")
		);
	}

	[Test]
	public void VerifyAccount_DatabaseVerificationFails_ReturnsInternalServerError()
	{
		var mockAccount = new Account { Email = "test@example.com" };
		_mockDatabaseService.Setup(db => db.GetAccountByEmail(It.IsAny<string>())).Returns(mockAccount);
		_mockDatabaseService.Setup(db => db.CheckCode(It.IsAny<string>(), It.IsAny<string>())).Returns("true");
		_mockDatabaseService.Setup(db => db.VerifyAccountEmail(It.IsAny<string>())).Returns("failure");
		var request = new VerifyCodeRequest { Email = "test@example.com", Code = "123456" };

		var result = _controller.VerifyAccount(request);

		Assert.That(result, Is.InstanceOf<ObjectResult>());
		var serverErrorResult = result as ObjectResult;
		Assert.That(serverErrorResult?.StatusCode, Is.EqualTo(500));
		Assert.That(
			serverErrorResult?.Value?.ToString(),
			Does.Contain("Failed when writing code to database")
		);
	}

	[Test]
	public void VerifyAccount_EmailSendingFails_ReturnsInternalServerError()
	{
		var mockAccount = new Account { Email = "test@example.com", FirstName = "John", LastName = "Doe" };
		_mockDatabaseService.Setup(db => db.GetAccountByEmail(It.IsAny<string>())).Returns(mockAccount);
		_mockDatabaseService.Setup(db => db.CheckCode(It.IsAny<string>(), It.IsAny<string>())).Returns("true");
		_mockDatabaseService.Setup(db => db.VerifyAccountEmail(It.IsAny<string>())).Returns("success");
		_mockEmailService.Setup(email => email.SendVerifiedConfirmationEmail(It.IsAny<string>(), It.IsAny<string>())).Returns("failure");
		var request = new VerifyCodeRequest { Email = "test@example.com", Code = "123456" };

		var result = _controller.VerifyAccount(request);

		Assert.That(result, Is.InstanceOf<ObjectResult>());
		var serverErrorResult = result as ObjectResult;
		Assert.That(serverErrorResult?.StatusCode, Is.EqualTo(500));
		Assert.That(
			serverErrorResult?.Value?.ToString(),
			Does.Contain("Failed when sending Email")
		);
	}

	[Test]
	public void VerifyAccount_SuccessfulExecution_ReturnsOk()
	{
		var mockAccount = new Account { Email = "test@example.com", FirstName = "John", LastName = "Doe" };
		_mockDatabaseService.Setup(db => db.GetAccountByEmail(It.IsAny<string>())).Returns(mockAccount);
		_mockDatabaseService.Setup(db => db.CheckCode(It.IsAny<string>(), It.IsAny<string>())).Returns("true");
		_mockDatabaseService.Setup(db => db.VerifyAccountEmail(It.IsAny<string>())).Returns("success");
		_mockEmailService.Setup(email => email.SendVerifiedConfirmationEmail(It.IsAny<string>(), It.IsAny<string>())).Returns("success");
		var request = new VerifyCodeRequest { Email = "test@example.com", Code = "123456" };

		var result = _controller.VerifyAccount(request);

		Assert.That(result, Is.InstanceOf<OkObjectResult>());
		var okResult = result as OkObjectResult;
		Assert.That(
			okResult?.Value?.GetType().GetProperty("message")?.GetValue(okResult.Value, null),
			Is.EqualTo("success")
		);
	}

	[Test]
	public void Login_RequestBodyMissingEmailOrPassword_ReturnsBadRequest()
	{
		var request = new LoginRequest { email = "", password = "" };

		var result = _controller.Login(request);

		Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
		var badRequestResult = result as BadRequestObjectResult;
		Assert.That(
			badRequestResult?.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value, null),
			Is.EqualTo("Data incomplete, check request body.")
		);
	}

	[Test]
	public void Login_AccountNotFound_ReturnsBadRequest()
	{
		_mockDatabaseService.Setup(db => db.GetAccountByEmail(It.IsAny<string>())).Returns((Account)null);
		var request = new LoginRequest { email = "test@example.com", password = "password123" };

		var result = _controller.Login(request);

		Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
		var badRequestResult = result as BadRequestObjectResult;
		Assert.That(
			badRequestResult?.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value, null),
			Is.EqualTo("Incorrect log in details")
		);
	}

	[Test]
	public void Login_IncorrectPassword_ReturnsBadRequest()
	{
		var mockAccount = new Account { Email = "test@example.com", Password = "correcthash" };
		_mockDatabaseService.Setup(db => db.GetAccountByEmail(It.IsAny<string>())).Returns(mockAccount);
		var request = new LoginRequest { email = "test@example.com", password = "wrongpassword" };

		var result = _controller.Login(request);

		Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
		var badRequestResult = result as BadRequestObjectResult;
		Assert.That(
			badRequestResult?.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value, null),
			Is.EqualTo("Incorrect log in details")
		);
	}

	[Test]
	public void Login_RoleNotFound_ReturnsForbidden()
	{
		var mockAccount = new Account { ID = 1, Email = "test@example.com", Password = "hashedpassword" };
		_mockDatabaseService.Setup(db => db.GetAccountByEmail(It.IsAny<string>())).Returns(mockAccount);
		_mockDatabaseService.Setup(db => db.GetAccountRoleName(mockAccount)).Returns(string.Empty);
		var request = new LoginRequest { email = "test@example.com", password = "password123" };

		var result = _controller.Login(request);

		Assert.That(result, Is.InstanceOf<ObjectResult>());
		var forbiddenResult = result as ObjectResult;
		Assert.That(forbiddenResult?.StatusCode, Is.EqualTo(403));
		Assert.That(
			forbiddenResult?.Value?.GetType().GetProperty("message")?.GetValue(forbiddenResult.Value, null),
			Is.EqualTo("Role not found.")
		);
	}

	[Test]
	public void Login_DatabaseFailsToAssignToken_ReturnsBadRequest()
	{
		var mockAccount = new Account { ID = 1, Email = "test@example.com", Password = "hashedpassword" };
		_mockDatabaseService.Setup(db => db.GetAccountByEmail(It.IsAny<string>())).Returns(mockAccount);
		_mockDatabaseService.Setup(db => db.GetAccountRoleName(mockAccount)).Returns("User");
		_mockDatabaseService.Setup(db => db.AssignToken(It.IsAny<int>(), It.IsAny<string>())).Returns("false");
		var request = new LoginRequest { email = "test@example.com", password = "password123" };

		var result = _controller.Login(request);

		Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
		var badRequestResult = result as BadRequestObjectResult;
		Assert.That(
			badRequestResult?.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value, null),
			Is.EqualTo("Failed to assign token during login")
		);
	}

	[Test]
	public void Login_SuccessfulLogin_ReturnsOk()
	{
		var mockAccount = new Account { ID = 1, Email = "test@example.com", Password = "hashedpassword" };
		_mockDatabaseService.Setup(db => db.GetAccountByEmail(It.IsAny<string>())).Returns(mockAccount);
		_mockDatabaseService.Setup(db => db.GetAccountRoleName(mockAccount)).Returns("User");
		_mockDatabaseService.Setup(db => db.AssignToken(It.IsAny<int>(), It.IsAny<string>())).Returns("true");
		var request = new LoginRequest { email = "test@example.com", password = "password123" };

		var result = _controller.Login(request);

		Assert.That(result, Is.InstanceOf<ObjectResult>());
		var okResult = result as OkObjectResult;
		dynamic? responseObject = okResult.Value;

		Assert.That((string)responseObject.token, Is.Not.Null);
		Assert.That((string)responseObject.role, Is.EqualTo("User"));
	}

	[Test]
	public void VerifyToken_RequestIsNull_ReturnsBadRequest()
	{
		var result = _controller.VerifyToken(null);

		Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
		var badRequestResult = result as BadRequestObjectResult;
		Assert.That(
			badRequestResult?.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value, null),
			Is.EqualTo("Request is null")
		);
	}

	[Test]
	public void VerifyToken_TokenIsMissingOrEmpty_ReturnsBadRequest()
	{
		var request = new VerifyTokenRequest { token = "" };

		var result = _controller.VerifyToken(request);

		Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
		var badRequestResult = result as BadRequestObjectResult;
		Assert.That(
			badRequestResult?.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value, null),
			Is.EqualTo("Token is missing or empty")
		);
	}

	[Test]
	public void VerifyToken_TokenInvalidLength_ReturnsBadRequest()
	{
		var request = new VerifyTokenRequest { token = "tooshort" };

		var result = _controller.VerifyToken(request);

		Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
		var badRequestResult = result as BadRequestObjectResult;
		Assert.That(
			badRequestResult?.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value, null),
			Is.EqualTo("Token is in an invalid format")
		);
	}

	[Test]
	public void VerifyToken_TokenInvalid_ReturnsBadRequest()
	{
		var request = new VerifyTokenRequest { token = new string('a', 64) };
		_mockDatabaseService.Setup(db => db.VerifyToken(It.IsAny<string>())).Returns((string)null);

		var result = _controller.VerifyToken(request);

		Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
		var badRequestResult = result as BadRequestObjectResult;
		Assert.That(
			badRequestResult?.Value?.GetType().GetProperty("message")?.GetValue(badRequestResult.Value, null),
			Is.EqualTo("Token is invalid")
		);
	}

	[Test]
	public void VerifyToken_TokenValid_ReturnsOk()
	{
		var request = new VerifyTokenRequest { token = new string('a', 64) };
		_mockDatabaseService.Setup(db => db.VerifyToken(It.IsAny<string>())).Returns("valid");

		var result = _controller.VerifyToken(request);

		Assert.That(result, Is.InstanceOf<OkObjectResult>());
		var okResult = result as OkObjectResult;
		Assert.That(
			okResult?.Value?.GetType().GetProperty("message")?.GetValue(okResult.Value, null),
			Is.EqualTo("Token is valid")
		);
	}

	[Test]
	public void VerifyToken_UnhandledException_ReturnsInternalServerError()
	{
		var request = new VerifyTokenRequest { token = new string('a', 64) };
		_mockDatabaseService.Setup(db => db.VerifyToken(It.IsAny<string>())).Throws(new Exception("Database error"));

		var result = _controller.VerifyToken(request);

		Assert.That(result, Is.InstanceOf<ObjectResult>());
		var serverErrorResult = result as ObjectResult;
		Assert.That(serverErrorResult?.StatusCode, Is.EqualTo(500));
		Assert.That(
			serverErrorResult?.Value?.ToString(),
			Does.Contain("An error occurred while processing your request")
		);
	}

	[Test]
	public void VerifyUserToken_AuthorizationHeaderMissing_ReturnsUnauthorized()
	{
		var result = _controller.VerifyUserToken("validUserToken", null);

		Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
		var unauthorizedResult = result as UnauthorizedObjectResult;
		Assert.That(
			unauthorizedResult?.Value?.GetType().GetProperty("message")?.GetValue(unauthorizedResult.Value, null),
			Is.EqualTo("Authorization header is missing or invalid.")
		);
	}

	[Test]
	public void VerifyUserToken_InvalidInventoryKey_ReturnsUnauthorized()
	{
		var result = _controller.VerifyUserToken("validUserToken", "Bearer invalidkey");

		Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
		var unauthorizedResult = result as UnauthorizedObjectResult;
		Assert.That(
			unauthorizedResult?.Value?.GetType().GetProperty("message")?.GetValue(unauthorizedResult.Value, null),
			Is.EqualTo("Invalid Inventory Key.")
		);
	}

	[Test]
	public void VerifyUserToken_UserApiTokenNotFound_ReturnsNotFound()
	{
		_mockDatabaseService.Setup(db => db.GetCityRoleFromToken(It.IsAny<string>())).Returns((CityRoleDTO)null);

		var result = _controller.VerifyUserToken("invalidUserToken", ValidAuthorizationHeader);

		Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
		var notFoundResult = result as NotFoundObjectResult;
		Assert.That(
			notFoundResult?.Value?.GetType().GetProperty("message")?.GetValue(notFoundResult.Value, null),
			Is.EqualTo("User API Token is invalid or not found.")
		);
	}

	[Test]
	public void VerifyUserToken_ValidToken_ReturnsOk()
	{
		var mockCityRole = new CityRoleDTO { Role = "Admin", City = "New York" };
		_mockDatabaseService.Setup(db => db.GetCityRoleFromToken(It.IsAny<string>())).Returns(mockCityRole);

		var result = _controller.VerifyUserToken("validUserToken", ValidAuthorizationHeader);

		Assert.That(result, Is.InstanceOf<OkObjectResult>());
		var okResult = result as OkObjectResult;
		Assert.That(
			okResult?.Value?.GetType().GetProperty("role")?.GetValue(okResult.Value, null),
			Is.EqualTo("Admin")
		);
		Assert.That(
			okResult?.Value?.GetType().GetProperty("city")?.GetValue(okResult.Value, null),
			Is.EqualTo("New York")
		);
	}

	[Test]
	public void VerifyUserToken_UnhandledException_ReturnsInternalServerError()
	{
		_mockDatabaseService.Setup(db => db.GetCityRoleFromToken(It.IsAny<string>())).Throws(new Exception("Database error"));

		var result = _controller.VerifyUserToken("validUserToken", ValidAuthorizationHeader);

		Assert.That(result, Is.InstanceOf<ObjectResult>());
		var serverErrorResult = result as ObjectResult;
		Assert.That(serverErrorResult?.StatusCode, Is.EqualTo(500));
		Assert.That(
			serverErrorResult?.Value?.ToString(),
			Does.Contain("An error occurred while processing your request")
		);
	}

	[Test]
	public void GenerateVerificationCode_OutputIsSixCharacters()
	{
		var verificationCode = AuthController.GenerateVerificationCode();

		Assert.That(verificationCode.Length, Is.EqualTo(6));
	}

	[Test]
	public void GenerateVerificationCode_OutputContainsOnlyDigits()
	{
		var verificationCode = AuthController.GenerateVerificationCode();
		var regexResult = Regex.IsMatch(verificationCode, "^[0-9]{6}$");

		Assert.That(regexResult, Is.EqualTo(true));
	}

	[Test]
	public void GenerateVerificationCode_OutputIsInExpectedRange()
	{
		int verificationCode = int.Parse(AuthController.GenerateVerificationCode());

		Assert.That(verificationCode, Is.InRange(100000, 999999));
	}

	[Test]
	public void GenerateToken_OutputIsOfSpecifiedLength()
	{
		var token = AuthController.GenerateToken(64);

		Assert.That(token.Length, Is.EqualTo(64));
	}

	[Test]
	public void GenerateToken_OutputContainsOnlyAllowedCharacters()
	{
		var token = AuthController.GenerateToken(64);
		var regexResult = Regex.IsMatch(token, "^[A-Za-z0-9]+$");

		Assert.That(regexResult, Is.EqualTo(true));
	}

	[Test]
	public void GenerateToken_MultipleCallsProduceUniqueOutputs()
	{
		// No check is actually performed when generating each token to see if it already exists, 
		// we are working on the assumption that the chances of two codes being created are so small as to not matter
		// and codes only matter for the account they are created for and reset on every log in
		var token1 = AuthController.GenerateToken();
		var token2 = AuthController.GenerateToken();

		Assert.That(token1, Is.Not.EqualTo(token2));
	}
}