namespace AuthAPI.Models
{
	public class MemberAddress
	{
		public int ID { get; set; }
		public string FirstLine { get; set; }
		public string SecondLine { get; set; }
		public string ThirdLine { get; set; }
		public string FourthLine { get; set; }
		public string City { get; set; }
		public string County { get; set; }
		public string Country { get; set; }
		public string PostCode { get; set; }
	}
}
