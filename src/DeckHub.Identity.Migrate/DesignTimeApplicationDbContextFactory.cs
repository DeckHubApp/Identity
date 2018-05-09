using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DeckHub.Identity.Data;

namespace DeckHub.Identity.Migrate
{
    public class DesignTimeApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public const string LocalPostgres = "Host=localhost;Database=identity;Username=deckhub;Password=secretsquirrel";
        public static readonly string MigrationAssemblyName =
            typeof(DesignTimeApplicationDbContextFactory).Assembly.GetName().Name;

        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(args.FirstOrDefault() ?? LocalPostgres, b => b.MigrationsAssembly(MigrationAssemblyName));
            
            return new ApplicationDbContext(builder.Options);
        }
    }
}