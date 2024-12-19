using AuthAPI.Interfaces;
using AuthAPI.Services;
using MailKit.Net.Smtp;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDatabaseService, DatabaseService>();

builder.Services.AddScoped<IDbConnection>(provider =>
{
	var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
	return new SqlConnection(connectionString);
});

builder.Services.AddScoped<IFileWrapper, FileWrapper>();
builder.Services.AddScoped<ISmtpClient, SmtpClient>();
builder.Services.AddScoped<IEmailService>(provider =>
{
	var configuration = provider.GetRequiredService<IConfiguration>();
	var env = provider.GetRequiredService<IWebHostEnvironment>();

	var smtpConfig = configuration.GetSection("SmtpServer");

	string smtpUrl = smtpConfig.GetValue<string>("SmtpUrl");
	int smtpPort = smtpConfig.GetValue<int>("SmtpPort");
	string smtpEmail = smtpConfig.GetValue<string>("Email");
	string smtpPassword = smtpConfig.GetValue<string>("Password");
	IFileWrapper fileWrapper = provider.GetRequiredService<IFileWrapper>();
	ISmtpClient smtpClient = provider.GetRequiredService<ISmtpClient>();

	return new EmailService(smtpUrl, smtpPort, smtpEmail, smtpPassword, env.ContentRootPath, fileWrapper, smtpClient);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
