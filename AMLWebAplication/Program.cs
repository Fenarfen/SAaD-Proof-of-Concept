using AMLWebAplication.Components;
using AMLWebAplication.Models;
using AMLWebAplication.Services;
using Blazored.SessionStorage;
using Microsoft.Extensions.DependencyInjection;

namespace AMLWebAplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<IMonitorService, MonitorService>();
            builder.Services.AddScoped<IMediaLoanService, MediaLoanService>();

            builder.Services.AddScoped<ReportApiService>();
            builder.Services.AddHttpClient<LoanApiService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001");
            });
            builder.Services.AddSingleton<LoanApiService>(); 
            
            builder.Services.AddHttpClient();

            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddSingleton<NetworkMonitorService>();
            builder.Services.AddSingleton<SystemMonitorService>();
            builder.Services.AddSingleton<HttpClientService>();
			builder.Services.AddSingleton<LoginStateService>();
            builder.Services.AddBlazoredSessionStorage();
            builder.Services.AddHttpClient();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
