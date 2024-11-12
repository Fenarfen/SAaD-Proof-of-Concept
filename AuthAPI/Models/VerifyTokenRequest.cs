namespace AuthAPI.Models
{
	public class VerifyTokenRequest
	{
		public int accountID { get; set; }
		public string token { get; set; }
	}
}
