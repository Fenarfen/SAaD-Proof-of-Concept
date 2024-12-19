using Moq;
using NUnit.Framework;
using System.IO;
using MailKit.Net.Smtp;
using MimeKit;
using AuthAPI.Interfaces;
using AuthAPI.Services;
using Assert = NUnit.Framework.Assert;

namespace AuthAPI.Tests;

[TestFixture]
public class EmailServiceTests
{
	private Mock<ISmtpClient> _mockSmtpClient;
	private Mock<IFileWrapper> _mockFileWrapper;
	private EmailService _emailService;
	private string _smtpUrl = "smtp.test.com";
	private int _smtpPort = 587;
	private string _smtpEmail = "test@test.com";
	private string _smtpPassword = "password";
	private string _rootPath = "/root";

	[SetUp]
	public void Setup()
	{
		_mockSmtpClient = new Mock<ISmtpClient>();
		_mockFileWrapper = new Mock<IFileWrapper>();

		_emailService = new EmailService(
			_smtpUrl, _smtpPort, _smtpEmail, _smtpPassword, _rootPath,
			_mockFileWrapper.Object, _mockSmtpClient.Object);
	}

	[Test]
	public void SendVerificationEmail_ReturnsSuccess_WhenEmailSent()
	{
		// Arrange
		string emailContent = "<html>{{VERIFICATION_CODE}}</html>";
		_mockFileWrapper.Setup(f => f.ReadAllText(It.IsAny<string>())).Returns(emailContent);

		_mockSmtpClient.Setup(client => client.Connect(_smtpUrl, _smtpPort, false, default)).Verifiable();
		_mockSmtpClient.Setup(client => client.Authenticate(_smtpEmail, _smtpPassword, default)).Verifiable();
		_mockSmtpClient.Setup(client => client.Send(It.IsAny<MimeMessage>(), default, default))
					   .Returns("Email sent successfully");
		_mockSmtpClient.Setup(client => client.Disconnect(true, default)).Verifiable();

		// Act
		var result = _emailService.SendVerificationEmail("recipient@test.com", "123456");

		// Assert
		Assert.That(result, Is.EqualTo("success"));
		_mockSmtpClient.Verify(client => client.Connect(_smtpUrl, _smtpPort, false, default), Times.Once);
		_mockSmtpClient.Verify(client => client.Authenticate(_smtpEmail, _smtpPassword, default), Times.Once);
		_mockSmtpClient.Verify(client => client.Send(It.IsAny<MimeMessage>(), default, default), Times.Once);
		_mockSmtpClient.Verify(client => client.Disconnect(true, default), Times.Once);
	}

	[Test]
	public void SendVerificationEmail_ReturnsFailure_WhenEmailFailsToSend()
	{
		string emailContent = "<html>{{VERIFICATION_CODE}}</html>";
		_mockFileWrapper.Setup(f => f.ReadAllText(It.IsAny<string>())).Returns(emailContent);

		_mockSmtpClient.Setup(client => client.Connect(_smtpUrl, _smtpPort, false, default)).Verifiable();
		_mockSmtpClient.Setup(client => client.Authenticate(_smtpEmail, _smtpPassword, default)).Verifiable();
		_mockSmtpClient.Setup(client => client.Send(It.IsAny<MimeMessage>(), default, default))
					   .Returns("failed");
		_mockSmtpClient.Setup(client => client.Disconnect(true, default)).Verifiable();

		// Act
		var result = _emailService.SendVerificationEmail("recipient@test.com", "123456");

		Assert.That(result, Is.EqualTo("Failed to send verification email"));
		_mockSmtpClient.Verify(client => client.Connect(_smtpUrl, _smtpPort, false, default), Times.Once);
		_mockSmtpClient.Verify(client => client.Authenticate(_smtpEmail, _smtpPassword, default), Times.Once);
		_mockSmtpClient.Verify(client => client.Send(It.IsAny<MimeMessage>(), default, default), Times.Once);
		_mockSmtpClient.Verify(client => client.Disconnect(true, default), Times.Once);
	}

	[Test]
	public void SendVerifiedConfirmationEmail_ReturnsSuccess_WhenEmailSent()
	{
		// Arrange
		_mockFileWrapper.Setup(f => f.ReadAllText(It.IsAny<string>()))
			.Returns("<html>{{VERIFICATION_CODE}}</html>");

		_mockSmtpClient.Setup(client => client.Send(It.IsAny<MimeMessage>(), default, default)).Verifiable();

		// Act
		var result = _emailService.SendVerifiedConfirmationEmail("recipient@test.com", "John Doe");

		// Assert
		Assert.That(result, Is.EqualTo("success"));
		_mockSmtpClient.Verify(client => client.Send(It.IsAny<MimeMessage>(), default, default), Times.Once);
	}

	[Test]
	public void SendVerifiedConfirmationEmail_ReturnsFailure_WhenEmailFailsToSend()
	{
		// Arrange
		_mockFileWrapper.Setup(f => f.ReadAllText(It.IsAny<string>()))
			.Returns("<html>{{VERIFICATION_CODE}}</html>");

		_mockSmtpClient.Setup(client => client.Send(It.IsAny<MimeMessage>(), default, default))
			.Throws(new Exception("Failed to send email"));

		// Act
		var result = _emailService.SendVerifiedConfirmationEmail("recipient@test.com", "John Doe");

		// Assert
		Assert.That(result.Contains("Failed to send email"));
	}
}
