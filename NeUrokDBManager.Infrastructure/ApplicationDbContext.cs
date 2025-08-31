using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NeUrokDBManager.Core.Entities;

namespace NeUrokDBManager.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientColor> ClientColors { get; set; }
        public DbSet<Update> Updates { get; set; }


        public ApplicationDbContext()
        {
            //if (!Database.GetAppliedMigrations().Any())
            {
                if (Database.EnsureCreated())
                    Console.WriteLine("Database and tables created");
                else
                    Database.Migrate();
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

                if (!File.Exists(configPath))
                {
                    throw new FileNotFoundException("appsettings.json not found", configPath);
                }

                var json = File.ReadAllText(configPath);
                using var jsonDoc = JsonDocument.Parse(json);

                var connectionString = jsonDoc.RootElement
                    .GetProperty("ConnectionStrings")
                    .GetProperty("Main")
                    .GetString();

                optionsBuilder.UseMySql(
                    connectionString ?? throw new InvalidOperationException("Connection string is null"),
                    new MySqlServerVersion(new Version(8, 0, 36)));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ApplicationDbContext).Assembly);
        }

    }
}
