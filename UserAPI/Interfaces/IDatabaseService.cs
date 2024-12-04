using UserAPI.Models.Dtos;
using UserAPI.Models.Entities;

namespace UserAPI.Interfaces;

public interface IDatabaseService
{
	public string CreateMemberUser(AccountCreateDto account);
	public string EditAccount(ProfileManagementDTO account);
	public string CreateAddress(Address address);
	public string UpdateAddress(Address address);
	public string DeleteAddress(int addressID);
	public string DoesEmailExist(string email);
	public Account GetAccountByID(int accountID);
	public ProfileManagementDTO GetProfileManagementDTOfromToken(string token);
}
