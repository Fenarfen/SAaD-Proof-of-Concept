using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using UserAPI.Services;
using UserAPI.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Identity.Client;
using UserAPI.Models.Entities;
using UserAPI.Models.Dtos;
using System.Net.Http;
using System.Text.Json;

namespace UserAPI.Controllers
{
    [ApiController]
	[Route("api/[controller]")]
	public class AccountController : ControllerBase
	{
		private readonly IDatabaseService _databaseService;
		private HttpClient httpClient;

		public AccountController(IDatabaseService databaseService)
		{
			_databaseService = databaseService;

			httpClient = new()
			{
				BaseAddress = new Uri("http://host.docker.internal:32784/api/")
			};

			httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "testuserkey");
		}

		[HttpPost("create")]
		public async Task<IActionResult> CreateAccountAsync([FromBody] AccountCreateDto account)
		{
			if (account == null)
			{
				return BadRequest(new { message = "Account data is required." });
			}

			if (account.Password.IsNullOrEmpty() ||
				account.FirstName.IsNullOrEmpty() ||
				account.Email.IsNullOrEmpty() ||
				account.LastName.IsNullOrEmpty())
			{
				return UnprocessableEntity(new { message = "Account information is null or empty" });
			}

			// Validation of account object
			if (!IsValidEmail(account.Email))
			{
				return UnprocessableEntity(new { message = "Invalid email format." });
			}

			string resultOfEmailAlreadyExists = string.Empty;

			try
			{
				resultOfEmailAlreadyExists = _databaseService.DoesEmailExist(account.Email);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while processing your request. Please try again later." + ex.ToString() });
			}
			
			if (resultOfEmailAlreadyExists == "true")
			{
				return Conflict(new { message = "Email is already registered to an account." });
			}

			string resultOfCreateUser = string.Empty;

			try
			{
				resultOfCreateUser = _databaseService.CreateMemberUser(account);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while processing your request. Please try again later." + ex.ToString() });
			}

			if (resultOfCreateUser == null)
			{
				return BadRequest(new { message = "Account creation failed." });
			}

			HttpResponseMessage response = await httpClient.PostAsync($"auth/send-email-verification-code/{resultOfCreateUser}", null);
			response.EnsureSuccessStatusCode();
			
			return Ok( new { message = "Account created successfully, awaiting verification." });
		}

		[HttpPost("update")]
		public IActionResult UpdateAccount([FromBody] ProfileManagementDTO updatedAccount)
		{
			if (updatedAccount == null)
			{
                return BadRequest(new { message = "Expecting ProfileManagementDTO from body of the request" });
            }

			var currentUser = _databaseService.GetAccountByID(updatedAccount.ID);

			if (currentUser == null)
			{
				return NotFound(new { message = "User not found." });
			}

			if (!IsValidEmail(updatedAccount.Email))
			{
				return UnprocessableEntity(new { message = "Invalid email format." });
			}

			try
			{
				if(updatedAccount.Email != currentUser.Email)
				{
					string resultOfEmailAlreadyExists = _databaseService.DoesEmailExist(updatedAccount.Email);

					if (resultOfEmailAlreadyExists == "true")
					{
						return Conflict(new { message = "Email is already registered to an account." });
					}
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while processing your request. Please try again later." + ex.ToString() });
			}

			string resultOfUpdateUser = string.Empty;

			try
			{
				resultOfUpdateUser = _databaseService.EditAccount(updatedAccount);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while processing your request. Please try again later." + ex.ToString() });
			}

			if (resultOfUpdateUser != "success")
			{
				return BadRequest(new { message = "Account creation failed." });
			}

			return Ok(new { message = "Account updated successfully." });
		}

        [HttpGet("does-email-exist/{email}")]
		public IActionResult DoesEmailExist(string email)
		{
			if (email.IsNullOrEmpty())
			{
				return BadRequest(new { message = "No email address specified." });
			}

			if (!IsValidEmail(email))
			{
				return UnprocessableEntity(new { message = "Invalid email format." });
			}

			try
			{
				return Ok(new { exists = _databaseService.DoesEmailExist(email) });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while processing your request. Please try again later." + ex.ToString() });
			}
		}

		[HttpGet("get-profile-management-dto/{token}")]
		public IActionResult GetProfileManagementDTO(string token)
		{
			if (token.IsNullOrEmpty())
			{
				return BadRequest(new { message = "Token is missing." });
			}

			try
			{
				return Ok(new { profileManagementDTO = _databaseService.GetProfileManagementDTOfromToken(token) });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while processing your request. Please try again later." + ex.ToString() });
			}
		}

		public static bool IsValidEmail(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
				return false;

			try
			{
				//Check if the email address contains illegal characters and follows email address pattern e.g. example@email.com
				string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
				Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

				return regex.IsMatch(email);
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
