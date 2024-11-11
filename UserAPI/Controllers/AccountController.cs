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

namespace UserAPI.Controllers
{
    [ApiController]
	[Route("api/[controller]")]
	public class AccountController : ControllerBase
	{
		private readonly IDatabaseService _databaseService;

		public AccountController(IDatabaseService databaseService)
		{
			_databaseService = databaseService;
		}

		[HttpPost("create")]
		public IActionResult CreateAccount([FromBody] AccountCreateDto account)
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

			//Validation to account object
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

			if (resultOfCreateUser != "success")
			{
				return BadRequest(new { message = "Account creation failed." });
			}

			// Return a success response with the created account information
			return CreatedAtAction(nameof(CreateAccount), new { id = 1 }, new { message = "Account created successfully." });
		}

		[HttpPost("update/{id}")]
		public IActionResult UpdateAccount(string id, [FromBody] AccountUpdateDto updatedAccount)
		{
			if (id == null)
			{
				return BadRequest(new { message = "Account ID is required." });
			}

			int userID = -1;

			try
			{
				userID = Convert.ToInt32(id);
			}
			catch
			{
				return BadRequest(new { message = "ID given is not an integer." });
			}


			if (updatedAccount == null)
			{
				return BadRequest(new { message = "Account data is required." });
			}

			var currentUser = _databaseService.GetAccountByID(userID);

			if (currentUser == null)
			{
				return NotFound(new { message = "User not found." });
			}

			//Validate account info
			if (updatedAccount.Password.IsNullOrEmpty() ||
				updatedAccount.FirstName.IsNullOrEmpty() ||
				updatedAccount.Email.IsNullOrEmpty() ||
				updatedAccount.LastName.IsNullOrEmpty())
			{
				return UnprocessableEntity(new { message = "Updated account information is null or empty" });
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
				resultOfUpdateUser = _databaseService.EditAccount(userID, updatedAccount);
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
