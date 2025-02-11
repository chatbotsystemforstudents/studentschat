using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using Serilog;
using Tcp;
using WebApiGateway.Configuration;

namespace WebApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                  ?? builder.Configuration["EnvironmentName"]
                  ?? "Development";
            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
            builder.Environment.EnvironmentName = environment;

            // Add services to the container.
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Host.UseSerilog((context, configuration) =>
                                configuration.ReadFrom.Configuration(context.Configuration));
            builder.Services.AddScoped<Client>();
            builder.Services.AddSingleton(x => new ServerConfiguration(builder.Configuration["SpeechServer:Ip"], int.Parse(builder.Configuration["SpeechServer:Port"]!)));
            builder.Services.AddSingleton(x => new AppConfig() { 
                LipSyncOutputDir= builder.Configuration["LipSyncOutputDir"]!
            });


            var app = builder.Build();
            app.UseSwagger();
            app.UseSwaggerUI();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseCors(policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod());
            }
            else
            {
                app.UseCors(policy =>
                        policy.SetIsOriginAllowed(origin =>
                        {
                            // Allow http and https for localhost, and only https for the production domain
                            return (origin.StartsWith("http://localhost") ||
                                    origin.StartsWith("https://localhost")) ||
                                    origin.Contains("https://students-chat.org");
                        })
                        .AllowAnyHeader()
                        .AllowAnyMethod());

            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();
            Console.WriteLine("Starting server");
            app.Run();
        }
    }
}
