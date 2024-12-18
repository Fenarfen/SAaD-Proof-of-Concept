using UserAPI.Models.Entities;

namespace UserAPI.Models.Dtos
{
	public class ProfileManagementDTO
	{
		public int ID { get; set; }
		public int UserID { get; set; }
		public string Role { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime CreatedAt { get; set; }
		public bool Verified { get; set; }
		public List<Address> Addresses { get; set; } = new List<Address>();
	}
}
