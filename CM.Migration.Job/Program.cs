using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CM.Backend.Persistence.Model;
using CM.Migration.Job.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin.Logging;
using SimpleSoft.Mediator;
using SixLabors.ImageSharp;
using StructureMap;

namespace CM.Migration.Job
{
    class Program
    {
        private static IContainer Container = new Container(new Registry());
        
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var brandMigration = Container.GetInstance<BrandMigration>();
            var champagneMigration = Container.GetInstance<ChampagneMigration>();
            var userMigration = Container.GetInstance<UserMigration>();
            var tastingMigration = Container.GetInstance<TastingMigration>();
            var followersMigration = Container.GetInstance<FollowersMigration>();
            var brandFollowersMigration = Container.GetInstance<BrandFollowersMigration>();
            var userSavedBottleMigration = Container.GetInstance<UserSavedBottleMigration>();
                
            //Migrate Brand
            brandMigration.Execute().Wait();
            Console.WriteLine("**********");
            Console.WriteLine("Brands migrated --> Preparing for champagne Migration");
            Console.WriteLine("**********");
            Task.Delay(5000).Wait();
            Console.WriteLine("Starting migrating champagnes");
            champagneMigration.Execute().Wait();
            Console.WriteLine("**********");
            Console.WriteLine("Champagnes migrated --> Preparing for User Migration");
            Console.WriteLine("**********");
            Task.Delay(5000).Wait();
            Console.WriteLine("Starting migrating users");
            userMigration.Execute().Wait();
            Console.WriteLine("**********");
            Console.WriteLine("Users migrated --> Preparing for Tasting Migration");
            Console.WriteLine("**********");
            Task.Delay(5000).Wait();
            Console.WriteLine("Starting migrating Tastings");
            tastingMigration.Execute().Wait();
            Console.WriteLine("**********");
            Console.WriteLine("Tastings migrated --> Preparing for Followers Migration");
            Console.WriteLine("**********");
            Task.Delay(5000).Wait();
            Console.WriteLine("Starting migrating followers");
            followersMigration.Execute().Wait();
            brandFollowersMigration.Execute().Wait();
            Task.Delay(5000).Wait();
            Console.WriteLine("Starting migrate of savedBottles");
            userSavedBottleMigration.Execute().Wait();

        }
    }
}
