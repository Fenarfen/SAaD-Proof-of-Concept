using AuthAPI.Models.Entities;

namespace AuthAPI.Interfaces
{
	public interface IDatabaseService
	{
		public string AssignVerificationCode(int accountID, string code);
		public Account GetAccountByID(int accountID);
		public Account GetAccountByEmail(string email);
		public string CheckCode(int accountID, string code);
		public string VerifyAccount(int accountID);
		public string AssignToken(int id, string token);
	}
}
