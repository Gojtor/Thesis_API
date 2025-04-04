using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.OpenApi.Models;
using System;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Thesis_ASP.Resources;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Thesis_ASP
{
    public class TCGDbContext : DbContext
    {
        private readonly IHubContext<WebSocket> webSocket;

        public TCGDbContext(DbContextOptions<TCGDbContext> options, IHubContext<WebSocket> webSocket) : base(options)
        {
            this.webSocket = webSocket;
        }
        public DbSet<Card> Cards { get; set; }
        public DbSet<InGameCard> InGameCards { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Card>().ToTable("Cards");
            modelBuilder.Entity<InGameCard>().ToTable("InGameCards");
        }
        public async Task AddCardsFromJSON()
        {
            if (!Cards.Any())
            {
                try
                {
                    var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "cardsDB.json");
                    if (!File.Exists(jsonPath))
                    {
                        Console.WriteLine("Couldn't locate JSON file.");
                        return;
                    }

                    string jsonString = File.ReadAllText(jsonPath);

                    List<Card> cards = JsonConvert.DeserializeObject<List<Card>>(jsonString);
                    if (cards == null || !cards.Any())
                    {
                        Console.WriteLine("The JSON file is wrong");
                        return;
                    }
                    await AddRangeAsync(cards);
                    await SaveChangesAsync();
                    Console.WriteLine("JSON loaded to database succesfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while loading JSON: {ex.Message}");
                }
            }
        }

        public async Task AddEnemyCardsFromJSON()
        {
                try
                {
                    var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "enemyCards.json");
                    if (!File.Exists(jsonPath))
                    {
                        Console.WriteLine("Couldn't locate JSON file.");
                        return;
                    }

                    string jsonString = File.ReadAllText(jsonPath);

                    List<InGameCard> inGameCards = JsonConvert.DeserializeObject<List<InGameCard>>(jsonString);
                    if (inGameCards == null || !inGameCards.Any())
                    {
                        Console.WriteLine("The JSON file is wrong");
                        return;
                    }
                    await AddRangeAsync(inGameCards);
                    await SaveChangesAsync();
                    Console.WriteLine("JSON loaded to database succesfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while loading JSON: {ex.Message}");
                }
        }

        public async Task DeleteAllCards()
        {
            try
            {
                var allCards = Cards.ToList();
                Cards.RemoveRange(allCards);
                await SaveChangesAsync();
                Console.WriteLine("Cards table cleared");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro while clearing database: {ex.Message}");
            }
        }

        public async Task DeleteAllInGameCards()
        {
            try
            {
                var allInGameCards = InGameCards.ToList();
                InGameCards.RemoveRange(allInGameCards);
                await SaveChangesAsync();
                Console.WriteLine("InGameCards table cleared");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro while clearing database: {ex.Message}");
            }
        }

        public async Task SendMessageToClient(string gameID)
        {
            await webSocket.Clients.Group(gameID).SendAsync("ReceiveMessage", "Message: TEST FROM API");
        }

        public async Task SendEnemyConnectedToClient(string gameID)
        {
            
            await webSocket.Clients.Group(gameID).SendAsync("EnemyConnected", "Message: ENEMYCONNECTED FROM API","API_ENEMY");
        }

        public async Task SendYouAreFirstToPlayer(string gameID,string playerName)
        {
            await webSocket.Clients.Group(gameID).SendAsync("YouAreFirst", "Message: You are the first");
        }

    }
}
