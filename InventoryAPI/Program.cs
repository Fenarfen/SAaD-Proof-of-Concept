using InventoryAPI.Interfaces;
using InventoryAPI.Repositories;
using InventoryAPI.Services;
using StackExchange.Redis;

namespace InventoryAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddScoped<BranchRepository>();
        builder.Services.AddScoped<MediaRepository>();

        string connString = builder.Configuration.GetConnectionString("DefaultConnection")?? "";
        DatabaseService service = new(connString);

        builder.Services.AddSingleton<IDatabaseService>(service);

        connString = builder.Configuration.GetConnectionString("RedisConnection") ?? "";
        ConfigurationOptions configOptions = new()
        {
            EndPoints = { connString },
            AbortOnConnectFail = true,
        };
        RedisCacheService redisCacheService = new(configOptions);

        builder.Services.AddSingleton<ICacheService>(redisCacheService);

        builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
