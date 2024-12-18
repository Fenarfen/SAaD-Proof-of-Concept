using AuthAPI.Interfaces;
using AuthAPI.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDatabaseService>(provider =>
{
	string connString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
	return new DatabaseService(connString);
});

builder.Services.AddScoped<IEmailService>(provider =>
{
	var configuration = provider.GetRequiredService<IConfiguration>();
	var env = provider.GetRequiredService<IWebHostEnvironment>();

	var smtpConfig = configuration.GetSection("SmtpServer");

	string smtpUrl = smtpConfig.GetValue<string>("SmtpUrl");
	int smtpPort = smtpConfig.GetValue<int>("SmtpPort");
	string smtpEmail = smtpConfig.GetValue<string>("Email");
	string smtpPassword = smtpConfig.GetValue<string>("Password");

	return new EmailService(smtpUrl, smtpPort, smtpEmail, smtpPassword, env.ContentRootPath);
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
