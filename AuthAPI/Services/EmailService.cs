using AuthAPI.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using System.Diagnostics.CodeAnalysis;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace AuthAPI.Services;

public class EmailService : IEmailService
{
	private readonly string _smtpUrl;
	private readonly int _smtpPort;
	private readonly string _smtpEmail;
	private readonly string _smtpPassword;
	private readonly string _rootPath;

	public EmailService(string smtpUrl, int smtpPort, string smtpEmail, string smtpPassword, string rootPath)
	{
		_smtpUrl = smtpUrl;
		_smtpPort = smtpPort;
		_smtpEmail = smtpEmail;
		_smtpPassword = smtpPassword;
		_rootPath = rootPath;
	}

	public string SendVerificationEmail(string recipient, string code)
	{
		try
		{
			string filePath = Path.Combine(_rootPath, "Templates", "VerificationCodeEmail.html");

			string htmlBody = File.ReadAllText(filePath).Replace("{{VERIFICATION_CODE}}", code);

			string response = SendEmail(recipient, "Your Verification Code", htmlBody);

			if(response == "failed")
			{
				return "Failed to send verification email";
			}

			return "success";
		}
		catch (Exception ex)
		{
			return ex.ToString();
		}
	}

	public string SendVerifiedConfirmationEmail(string recipient, string name)
	{
		try
		{
			string filePath = Path.Combine(_rootPath, "Templates", "VerificationCompleteEmail.html");

			string htmlBody = File.ReadAllText(filePath).Replace("{{NAME}}", name);

			SendEmail(recipient, "You are verified!", htmlBody);

			return "success";
		}
		catch (Exception ex)
		{
			return ex.ToString();
		}
	}

	private string SendEmail(string recipientAddress, string subject, string body)
	{
		var message = new MimeMessage();
		message.From.Add(new MailboxAddress("test", _smtpEmail));
		message.To.Add(new MailboxAddress("test", recipientAddress));
		message.Subject = subject;
		message.Body = new TextPart("html")
		{
			Text = body
		};

		using (var client = new SmtpClient())
		{
			client.Connect(_smtpUrl, _smtpPort, false);
			client.Authenticate(_smtpEmail, _smtpPassword);
			string response = "failed";
			response = client.Send(message);
			client.Disconnect(true);

			if(response == "failed")
			{
				throw new Exception("Unknown error: Failed to send email");
			}

			return response;
		}
	}
}
