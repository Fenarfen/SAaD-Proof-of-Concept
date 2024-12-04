using AuthAPI.Models;
using AuthAPI.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPI.Interfaces;

public interface IEmailService
{
	public string SendVerificationEmail(string recipient, string code);
	public string SendVerifiedConfirmationEmail(string recipient, string name);
}
