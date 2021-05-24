using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VertMarket_programming_Challenge.DataAccess;
using VertMarket_programming_Challenge.Interfaces;
using VertMarket_programming_Challenge.Model;

namespace VertMarket_programming_Challenge
{
    class Program
    {

        static IReadOnlyDictionary<string, string> DefaultConfiguration { get; } =
           new Dictionary<string, string>()
           {
               ["MagezineStore:BaseUrl"] = "http://magazinestore.azurewebsites.net",
               ["MagezineStore:TimeOutInSeconds"] = "20"
           };

        static void Main(string[] args)
        {
            MainAsync(args).Wait();
        }

        static async Task MainAsync(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder();
                builder.AddInMemoryCollection(DefaultConfiguration);
                builder.AddCommandLine(args);

                var configurations = builder.Build();
                var serviceProvider = new ServiceCollection()
                    .Configure<ConfigurationModel>(configurations.GetSection("MagezineStore"))
                    .AddTransient<IMagazineStoreSa, DataAccessLayer>()
                    .BuildServiceProvider();


                var store = serviceProvider.GetService<IMagazineStoreSa>();
                var result = await store.IdentifySubcribersToAllCategories();
                Console.WriteLine($"Result: {JsonConvert.SerializeObject(result)}");
                
            }
            catch (Exception)
            {
                Console.WriteLine("An eror as occurred.");
            }


        }
    }
}
