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

namespace Thesis_ASP
{
    public class TCGDbContext : DbContext
    {
        public DbSet<Card> Cards { get; set; }
        public TCGDbContext(DbContextOptions<TCGDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
        public void Seed()
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

                    AddRange(cards);
                    SaveChanges();
                    Console.WriteLine("JSON loaded to database succesfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while loading JSON: {ex.Message}");
                }
            }
        }
        public void DeleteAllCards()
        {
            try
            {
                var allCards = Cards.ToList();
                Cards.RemoveRange(allCards);
                SaveChanges();
                Console.WriteLine("Database cleared");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro while clearing database: {ex.Message}");
            }
        }
    }
}
