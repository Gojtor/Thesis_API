using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.OpenApi.Models;
using System;
using System.Threading.Tasks;

namespace Thesis_ASP
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSignalR();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers();
            builder.Services.AddSingleton<GameGroupManager>();
            builder.Services.AddDbContext<TCGDbContext>(options =>
            options.UseMySql("Server=localhost;Port=3306;Database=egy_darab;User=lorxy;Password=lorand;",
            new MySqlServerVersion(new Version(10, 5, 9))
            ));


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TCGDbContext>();
            await dbContext.DeleteAllCards();
            await dbContext.DeleteAllInGameCards();
            await dbContext.AddCardsFromJSON();     
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapControllers();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<WebSocket>("/websocket");
                endpoints.MapControllers();
            });
            app.Run();
        }
    }
}
