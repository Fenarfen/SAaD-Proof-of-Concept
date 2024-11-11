namespace UserAPI.Models.Dtos
{
	public class AccountCreateDto
	{
		public required string Email { get; set; }
		public required string Password { get; set; }
		public required string FirstName { get; set; }
		public required string LastName { get; set; }
	}
}
