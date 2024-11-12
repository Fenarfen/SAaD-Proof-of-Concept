using AuthAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using AuthAPI.Models;
using AuthAPI.Models.Entities;
using Microsoft.AspNetCore.Authorization;

namespace AuthAPI.Controllers
{
	[ApiController]
	[Route("api/auth")]
	public class AuthController : ControllerBase
	{
		private readonly IConfiguration _config;
		private readonly IDatabaseService _databaseService;
		public AuthController(IConfiguration config, IDatabaseService databaseService)
		{
			_config = config;
			_databaseService = databaseService;
		}

		[HttpPost("start-verification/{accountID}")]
		public IActionResult AssignVerificationCode(int accountID)
		{
			//check user exists
			try
			{
				Account account = _databaseService.GetAccountByID(accountID);

				if (account == null)
				{
					return BadRequest(new { message = "User does not exist." });
				}

				if(account.Verified == true)
				{
					return BadRequest(new { message = "Account is already verified" });
				}

				string verificationCode = GenerateVerificationCode();

				//todo mail
				_databaseService.AssignVerificationCode(accountID, verificationCode);

				return Ok(new { message = "Verification started - " + verificationCode });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while processing your request. Please try again later." + ex.ToString() });
			}
		}

		[HttpPost("verify")]
		public IActionResult VerifyAccount([FromBody] VerifyCodeRequest verifyCodeRequest)
		{
			//check user exists
			try
			{
				if (_databaseService.GetAccountByID(verifyCodeRequest.id) == null)
				{
					return BadRequest(new { message = "User does not exist." });
				}

				//check code against latest code assigned to this userID
				if (_databaseService.CheckCode(verifyCodeRequest.id, verifyCodeRequest.Code) != "true")
				{
					return BadRequest(new { message = "Code is incorrect." });
				}

				_databaseService.VerifyAccount(verifyCodeRequest.id);

				//send email saying they're verified
				return Ok(new { message = "Account is now verified" });
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
				if(request.Email.IsNullOrEmpty() || request.Password.IsNullOrEmpty())
				{
					return BadRequest(new { message = "Data incomplete, check request body." });
				}

				Account account = _databaseService.GetAccountByEmail(request.Email);

				if (account == null)
				{
					return BadRequest(new { message = "Incorrect log in details" });
				}

				if (request.Password != account.Password)
				{
					return BadRequest(new { message = "Incorrect log in details" });
				}

				string token = GenerateToken();
				_databaseService.AssignToken(account.ID, token);

				return Ok(new { token = token });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while processing your request. Please try again later. " + ex.ToString() });
			}
		}

		[HttpPost("Verify-token")]
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

			if (_databaseService.GetAccountByID(request.accountID) == null)
			{
				return BadRequest(new { message = "Account doesn't exist" });
			}

			try
			{
				string result = _databaseService.VerifyToken(request);

				if (result == "valid")
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

		private string GenerateVerificationCode()
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
