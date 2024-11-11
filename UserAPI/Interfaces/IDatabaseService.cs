using UserAPI.Models.Dtos;
using UserAPI.Models.Entities;

namespace UserAPI.Interfaces;

public interface IDatabaseService
{
	public string CreateMemberUser(Account account);
	public string EditAccount(int id, AccountUpdateDto account);
	public string CreateMemberAddress(MemberAddress memberAddress);
	public string EditMemberAddress(int id, MemberAddress memberAddress);
	public string DeleteMemberAddress(int addressID);
	public string DoesEmailExist(string email);
	public Account GetAccountByID(int accountID);
}
