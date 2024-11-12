using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using UserAPI.Services;
using UserAPI.Interfaces;
using UserAPI.Models.Entities;
using UserAPI.Models.Dtos;

namespace UserAPI.Controllers
{
    [ApiController]
	[Route("api/[controller]")]
	public class MemberAddressController : ControllerBase
	{
		private readonly IDatabaseService _databaseService;

		public MemberAddressController(IDatabaseService databaseService)
		{
			_databaseService = databaseService;
		}

		[HttpPost("create")]
		public IActionResult CreateMemberAddress([FromBody] AddressCreateUpdateDto memberAddress)
		{
			if (memberAddress == null)
			{
				return BadRequest(new { message = "Address data is required." });
			}

			try
			{
				Account account = _databaseService.GetAccountByID(memberAddress.AccountID);

				if(account == null)
				{
					return BadRequest(new { message = "Account ID doesn't exist" });
				}

				_databaseService.CreateMemberAddress(memberAddress);

				return Ok(new { message = "Address created successfully." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while processing your request. Please try again later." + ex.ToString() });
			}

			// Return a success response with the created address information
			
		}

		[HttpPost("edit/{addressID}")]
		public IActionResult EditMemberAddress(int addressID, [FromBody] Address memberAddress)
		{
			if (memberAddress == null)
			{
				return BadRequest(new { message = "Address data is required." });
			}

			string resultOfEditMemberAddress = string.Empty;

			try
			{
				resultOfEditMemberAddress = _databaseService.EditMemberAddress(addressID, memberAddress);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while processing your request. Please try again later." + ex.ToString() });
			}

			// Return a success response with the created account information
			return Ok(new { message = "Address updated successfully." });
		}

		[HttpDelete("delete/{addressId}")]
		public IActionResult DeleteMemberAddress(int addressId)
		{
			try
			{
				Convert.ToInt32(addressId);
			}
			catch 
			{
				return BadRequest(new { message = "Address ID given should be an integer." });
			}

			try
			{
				string result = _databaseService.DeleteMemberAddress(addressId);

				if (result == "not found")
				{
					return BadRequest(new { message = "No address found with that id" });
				}
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while processing your request. Please try again later." + ex.ToString() });
			}

			return Ok(new { message = "Address has been deleted" });
		}
	}
}
