using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace OrderApi.Data
{
    public static class SeedData
    {
        public static void EnsureCreated(OrdersContext context)
        {
            System.Console.WriteLine("Creating database...");
            context.Database.EnsureCreated();
            RelationalDatabaseCreator databaseCreator =(RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>();
            databaseCreator.CreateTables();


            System.Console.WriteLine("Database and tables' creation complete.....");
        }
    }
}
