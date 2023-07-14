using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QIA.Plugin.OpcClient.Database;
using QIA.Plugin.OpcClient.Repository;
using QIA.Plugin.OpcClient.Services;
using QIA.Plugin.OpcClient.Services.Interfaces;
using System;
using System.IO;
using System.Text.Json;

namespace QIA.Plugin.OpcClient.Core
{
    public sealed class Extensions
    {
        private static Extensions _instance;
        private static IConfiguration config;
        private static ServiceProvider serviceProvider;
        private Extensions()
        {
            config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("qia.opc/appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }

        private static Extensions GetInstance()
        {
            _instance ??= new Extensions();
            return _instance;
        }

        public static void InjectServices()
        {
            LoggerManager.InitLogging();

            GetInstance();

            serviceProvider = new ServiceCollection()
                //appsettings
                .Configure<AppSettings>(config.GetSection(nameof(AppSettings)))
                .Configure<AzureEventHub>(config.GetSection(nameof(AzureEventHub)))
                //db
                .AddDbContext<OpcDbContext>(x =>
                    x.UseSqlServer(config.GetConnectionString("DefaultConnection")))
                //services
                .AddTransient<IDataAccess, DataAccess>()
                .AddScoped<ISubscriptionManager, SubscriptionManager>()
                .AddTransient<ITreeManager, TreeManager>()
                .AddTransient<INodeManager, NodeManager>()
                .AddTransient<IAzureMessageService, AzureMessageService>()
                .BuildServiceProvider();

            var scope = serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetRequiredService<OpcDbContext>();
            try
            {
                var appSettings = ReadSettings<AppSettings>();
                Console.WriteLine($"AppSettings.json:" +
                    JsonSerializer.Serialize(appSettings,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web)
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,

                    }).Replace("\"", ""));

                var canConnect = context.Database.CanConnect();
                LoggerManager.Logger.Information($"Db connection {(canConnect ? "successfull" : "failed")}");

                if (appSettings.RecreateDb)
                {
                    Console.WriteLine("Are you sure you want to delete and recreate db (y/N):");
                    if (Console.ReadLine().ToString().ToLower().Trim() == "y")
                    {
                        context.Database.EnsureDeleted();
                    }
                }

                var created = context.Database.EnsureCreated();
                LoggerManager.Logger.Information($"Db {(created ? "is recreated" : "already exists")}");
                //OpcDbContextSeed.Seed(context);
            }
            catch (Exception ex)
            {
                LoggerManager.Logger.Information($"Db: {context.Database.GetConnectionString()}");
                LoggerManager.Logger.Error("SQL connection issue appeared: {0}, {1}", ex.Message, ex.InnerException?.Message);
                throw;
            }
        }
        public static string ReadServerUrl() => config.GetConnectionString("OpcUrl");
        public static T ReadSettings<T>() where T : class => serviceProvider.GetRequiredService<IOptions<T>>().Value;
        public static T GetService<T>() where T : class => serviceProvider.GetRequiredService<T>();
    }
}
