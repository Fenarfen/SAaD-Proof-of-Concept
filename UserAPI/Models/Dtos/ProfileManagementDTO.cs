using UserAPI.Models.Entities;

namespace UserAPI.Models.Dtos
{
	public class ProfileManagementDTO
	{
		public required int ID { get; set; }
		public required string Role { get; set; }
		public required string Email { get; set; }
		public required string FirstName { get; set; }
		public required string LastName { get; set; }
		public required DateTime CreatedAt { get; set; }
		public required bool Verified { get; set; }
		public List<Address> Addresses { get; set; } = new List<Address>();
	}
}
