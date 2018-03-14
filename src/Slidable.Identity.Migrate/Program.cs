using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql;
using RendleLabs.EntityFrameworkCore.MigrateHelper;

namespace Slidable.Identity.Migrate
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var loggerFactory = new LoggerFactory().AddConsole((_, level) => true);
            Console.WriteLine("Trying Migration...");
            await new MigrationHelper(loggerFactory).TryMigrate(args);
            Console.WriteLine("Done.");
        }
    }
}
