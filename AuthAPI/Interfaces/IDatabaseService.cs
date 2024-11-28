using AuthAPI.Models;
using AuthAPI.Models.DTOs;
using AuthAPI.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPI.Interfaces;

public interface IDatabaseService
{
	public string StoreVerificationCode(int accountID, string code);
	public Account GetAccountByID(int accountID);
	public Account GetAccountByEmail(string email);
	public string CheckCode(int accountID, string code);
	public string VerifyAccountEmail(int accountID);
	public string AssignToken(int id, string token);
	public string VerifyToken(string request);
	public CityRoleDTO GetCityRoleFromToken(string token);
	public string GetAccountRoleName(Account account);
}
