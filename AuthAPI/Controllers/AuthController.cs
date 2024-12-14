using AuthAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using AuthAPI.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using AuthAPI.Models.Requests;
using AuthAPI.Models.DTOs;

namespace AuthAPI.Controllers
{
    [ApiController]
	[Route("api/auth")]
	public class AuthController : ControllerBase
	{
		private readonly IConfiguration _config;
		private readonly IDatabaseService _databaseService;
		private readonly IEmailService _emailService;
		private const string _inventoryAPIKey = "testinventorykey";
		private const string _userAPIKey = "testuserkey";
		private const string _reportAPIKey = "testreportkey";

		public AuthController(IConfiguration config, IDatabaseService databaseService, IEmailService emailService)
		{
			_config = config;
			_databaseService = databaseService;
			_emailService = emailService;
		}

		[HttpPost("send-email-verification-code/{accountID}")]
		public IActionResult SendEmailVerification(int accountID, [FromHeader] string Authorization)
		{
			try
			{
				if (string.IsNullOrEmpty(Authorization) || !Authorization.StartsWith("Bearer "))
				{
					return Unauthorized(new { message = "Authorization header is missing or invalid." });
				}

				var inventoryApiKey = Authorization.Substring("Bearer ".Length).Trim();
				if (inventoryApiKey != _userAPIKey)
				{
					return Unauthorized(new { message = "Invalid User Key." });
				}

				Account account = _databaseService.GetAccountByID(accountID);

				if (account == null)
				{
					return BadRequest(new { message = "User does not exist." });
				}

				if (account.Verified == true)
				{
					return BadRequest(new { message = "Account is already verified" });
				}

				string verificationCode = GenerateVerificationCode();

				string dbResult = _databaseService.StoreVerificationCode(accountID, verificationCode);

				if(dbResult != "success")
				{
					return StatusCode(500, new { message = "Failed when writing code to database" });
				}

				string emailResult = _emailService.SendVerificationEmail(account.Email, verificationCode);

				if(emailResult != "success")
				{
					return StatusCode(500, new { message = "Failed when sending Email\n\n" + emailResult });
				}

				return Ok(new { message = "success" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while processing your request. Please try again later." + ex.ToString() });
			}
		}

		[HttpPost("verify-account")]
		public IActionResult VerifyAccount([FromBody] VerifyCodeRequest verifyCodeRequest)
		{
			try
			{
				Account account = _databaseService.GetAccountByEmail(verifyCodeRequest.Email);

				if (account == null)
				{
					return BadRequest(new { message = "User does not exist." });
				}

				//check code against latest code assigned to this userID
				if (_databaseService.CheckCode(verifyCodeRequest.Email, verifyCodeRequest.Code) != "true")
				{
					return BadRequest(new { message = "Code is incorrect." });
				}

				string dbResult = _databaseService.VerifyAccountEmail(verifyCodeRequest.Email);

				if (dbResult != "success")
				{
					return StatusCode(500, new { message = "Failed when writing code to database" });
				}

				string emailResult = _emailService.SendVerifiedConfirmationEmail(account.Email, $"{account.FirstName} {account.LastName}");

				if (emailResult != "success")
				{
					return StatusCode(500, new { message = "Failed when sending Email\n\n" + emailResult });
				}

				return Ok(new { message = "success" });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while processing your request. Please try again later. " + ex.ToString() });
			}
		}

		[HttpPost("login")]
		public IActionResult Login([FromBody] LoginRequest request)
		{
			try
			{
				if (request.email.IsNullOrEmpty() || request.password.IsNullOrEmpty())
				{
					return BadRequest(new { message = "Data incomplete, check request body." });
				}

				Account account = _databaseService.GetAccountByEmail(request.email);

				if (account == null)
				{
					return BadRequest(new { message = "Incorrect log in details" });
				}

				var hash = SHA3_256.HashData(Encoding.ASCII.GetBytes(request.password));
				string stringHash = Encoding.ASCII.GetString(hash);
                if (stringHash != account.Password)
				{
					return BadRequest(new { message = "Incorrect log in details" });
				}

				string role = _databaseService.GetAccountRoleName(account);

				if (role.IsNullOrEmpty())
				{
					return StatusCode(StatusCodes.Status403Forbidden, new { message = "Role not found." });
				}

				string token = GenerateToken();
				string dbResult = _databaseService.AssignToken(account.ID, token);

				if(dbResult != "true")
				{
					return BadRequest(new { message = "Failed to assign token during login" });
				}

				return Ok(new { token = token, role });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while processing your request. Please try again later. " + ex.ToString() });
			}
		}

		[HttpPost("verify-token")]
		public IActionResult VerifyToken([FromBody] VerifyTokenRequest request)
		{
			if (request == null)
			{
				return BadRequest(new { message = "Request is null" });
			}

			if (request.token.IsNullOrEmpty())
			{
				return BadRequest(new { message = "Token is missing or empty" });
			}

			if (request.token.Length != 64)
			{
				return BadRequest(new { message = "Token is in an invalid format" });
			}

			try
			{
				string result = _databaseService.VerifyToken(request.token);

				if (result != null)
				{
					return Ok(new { message = "Token is valid" });
				}
				else
				{
					return BadRequest(new { message = "Token is invalid" });
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while processing your request. Please try again later. " + ex.ToString() });
			}
		}

		[HttpPost("get-city-role-by-token/{userApiToken}")]
		public IActionResult VerifyUserToken(string userApiToken, [FromHeader] string Authorization)
		{
			try
			{
				if (string.IsNullOrEmpty(Authorization) || !Authorization.StartsWith("Bearer "))
				{
					return Unauthorized(new { message = "Authorization header is missing or invalid." });
				}

				var inventoryApiKey = Authorization.Substring("Bearer ".Length).Trim();
				if (inventoryApiKey != _inventoryAPIKey)
				{
					return Unauthorized(new { message = "Invalid Inventory Key." });
				}

				CityRoleDTO cityRoleDTO = _databaseService.GetCityRoleFromToken(userApiToken);

				if (cityRoleDTO == null)
				{
					return NotFound(new { message = "User API Token is invalid or not found." });
				}

				return Ok(new
				{
					role = cityRoleDTO.Role,
					city = cityRoleDTO.City
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while processing your request. Please try again later. " + ex.ToString() });
			}
		}

		public static string GenerateVerificationCode()
		{
			return new Random().Next(100000, 1000000).ToString();
		}

		public static string GenerateToken(int length = 64)
		{
			string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

			var stringBuilder = new StringBuilder(length);
			using (var rng = RandomNumberGenerator.Create())
			{
				byte[] randomBytes = new byte[length];
				rng.GetBytes(randomBytes);

				foreach (var b in randomBytes)
				{
					stringBuilder.Append(chars[b % chars.Length]);
				}
			}

			return stringBuilder.ToString();
		}
	}
}
