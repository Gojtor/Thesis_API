using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.OpenApi.Models;
using System;
using System.Threading.Tasks;
using Thesis_ASP.Data;

namespace Thesis_ASP
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            string dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSignalR();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers();
            builder.Services.AddSingleton<GameGroupManager>();

            builder.Services.AddDbContext<TCGDbContext>(options =>
            options.UseMySql($"Server={dbHost};Port=3306;Database=egy_darab;User=lorxy;Password=lorand;",
            new MySqlServerVersion(new Version(10, 5, 9))
            ));

            builder.Services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<TCGDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.AddAuthentication()
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
            });
            builder.Services.AddAuthorization();
            builder.Services.AddControllersWithViews();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowTrusted",
                    builder =>
                    {
                        builder.WithOrigins("https://gojtor.github.io/Thesis-OPTCG/game/", "https://gojtor.github.io")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials()
                            .SetIsOriginAllowedToAllowWildcardSubdomains()
                            .WithExposedHeaders("Set-Cookie", "Content-Type", "Authorization", "X-Requested-With");
                    });
            });

            var app = builder.Build();

            using (var migration_scope = app.Services.CreateScope())
            {
                var db = migration_scope.ServiceProvider.GetRequiredService<TCGDbContext>();

                try
                {
                    db.Database.Migrate();
                }
                catch (Exception ex)
                {
                    return;
                }
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TCGDbContext>();
            await dbContext.DeleteAllCards();
            await dbContext.DeleteAllInGameCards();
            await dbContext.AddCardsFromJSON();

            app.UseCors("AllowTrusted");

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<WebSocket>("/websocket");
                endpoints.MapControllers();
            });
            app.Run();
        }
    }
}
