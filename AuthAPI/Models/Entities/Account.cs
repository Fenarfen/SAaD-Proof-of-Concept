namespace AuthAPI.Models.Entities
{
	public class Account
	{
		public int ID { get; set; }
		public int? TokenID { get; set; }
		public required int RoleID { get; set; }
		public required string Email { get; set; }
		public required string Password { get; set; }
		public required string FirstName { get; set; }
		public required string LastName { get; set; }
		public required DateTime Created { get; set; }
		public required bool Verified { get; set; }
	}
}
