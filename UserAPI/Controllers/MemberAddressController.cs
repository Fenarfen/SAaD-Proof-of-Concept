using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using UserAPI.Services;
using UserAPI.Interfaces;
using UserAPI.Models.Entities;

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
		public IActionResult CreateMemberAddressController([FromBody] MemberAddress memberAddress)
		{
			if (memberAddress == null)
			{
				return BadRequest(new { message = "Address data is required." });
			}

			string resultOfCreateMemberAddress = string.Empty;

			try
			{
				resultOfCreateMemberAddress = _databaseService.CreateMemberAddress(memberAddress);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while processing your request. Please try again later." + ex.ToString() });
			}

			// Return a success response with the created account information
			return CreatedAtAction(nameof(MemberAddress), new { id = 1 }, new { message = "Address created successfully." });
		}

		[HttpPost("edit/{addressID}")]
		public IActionResult EditMemberAddressController(int addressID, [FromBody] MemberAddress memberAddress)
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
			return Ok(new { message = "Address updated successfully.", result = resultOfEditMemberAddress });
		}

		[HttpDelete("delete/{addressId}")]
		public IActionResult DeleteMemberAddressController(int addressId)
		{
			try
			{
				Convert.ToInt32(addressId);
			}
			catch 
			{
				return BadRequest(new { message = "Address ID given should be an integer." });
			}

			string resultOfDeleteMemberAddress = string.Empty;

			try
			{
				resultOfDeleteMemberAddress = _databaseService.DeleteMemberAddress(addressId);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while processing your request. Please try again later." + ex.ToString() });
			}

			return NoContent();
		}
	}
}
