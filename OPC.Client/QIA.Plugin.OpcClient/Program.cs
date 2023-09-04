using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using QIA.Library.Worker;
using QIA.Plugin.OpcClient.Core;
using QIA.Plugin.OpcClient.Core.Settings;
using QIA.Plugin.OpcClient.Database;
using QIA.Plugin.OpcClient.Repository;
using QIA.Plugin.OpcClient.Services;
using QIA.Plugin.OpcClient.Services.Interfaces;
using SignalR.Client;
using System;
using System.IO;
using System.Text.Json;

namespace QIA.Plugin.OpcClient
{
    /// <summary>
    /// Opc.Worker configDir=C:\dev\QIA_Service\ProjectRollout\OPC_UA\bin-folder\config appSettings=Databases,Assemblies,Logging,Logins,Workers.OPC_UA appControl=appControl
    /// </summary>
    class Program : QIAWorker
    {
        private static IConfiguration qiaConfig;
        private static string section;
        private static IConfigurationRoot cfgRoot = null;
        public static string path = Path.Combine(Directory.GetCurrentDirectory(), "qia.opc\\appsettings.json");
        public static string path2 = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

        public Program(IConfiguration config, string section) : base(config, section)
        {
            Program.qiaConfig = config;
            Program.section = section;
        }

        static void Main(string[] args)
        {
            ServicesInit(args);
        }
        protected override bool Init()
        {
            ServicesInit(new string[0]);
            return true; ;
        }

        protected override void Execute()
        {
            base.Execute();
        }

        public override string QIAClassSignature => "..o?/\u0010?\u007f??5?]\u001f\n???\u000f?? ??\u007f??b?\n?\v";


        public static void ServicesInit(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
               Host.CreateDefaultBuilder(args)

           .ConfigureServices((hostContext, services) =>
            {
                LoggerManager.InitLogging();

                IAppSettings settings;
                if (qiaConfig != null)
                {
                    settings = new PluginConfiguration(qiaConfig, section);
                }
                else
                {
                    var config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile(Program.path, optional: false, reloadOnChange: true)
                        .AddJsonFile(Program.path2, optional: false, reloadOnChange: true)
                        .Build();

                    cfgRoot = config;

                    settings = new StandaloneConfiguration(config);
                }
                services.AddSingleton<IConfigurationRoot>(cfgRoot)
                    .AddSingleton<IAppSettings>(settings)
                    //db
                    .AddDbContext<OpcDbContext>(x =>
                        x.UseSqlServer(settings.DbConnectionString))
                    //services
                    .AddSingleton<SignalRService>()
                    .AddTransient<IDataAccess, DataAccess>()
                    .AddTransient<ITreeManager, TreeManager>()
                    .AddScoped<ISubscriptionManager, SubscriptionManager>()
                    .AddTransient<INodeManager, NodeManager>()
                    .AddTransient<IAzureMessageService, AzureMessageService>();

                services.AddHostedService<SignalrHosted>()
                        .AddHostedService<AppHosted>();

                services.AddHttpClient<MessageService>();

                services.AddTransient<IOptions<IAppSettings>>(provider =>
                    Options.Create(provider.GetRequiredService<IAppSettings>()));

                var serviceProvider = services.BuildServiceProvider();
                Extensions.InjectProvider(services);

                var scope = serviceProvider.CreateScope();
                using var context = scope.ServiceProvider.GetRequiredService<OpcDbContext>();
                try
                {
                    var appSettings = Extensions.ReadSettings();
                    Console.WriteLine($"AppSettings.json:" +
                        JsonSerializer.Serialize(appSettings,
                        new JsonSerializerOptions(JsonSerializerDefaults.Web)
                        {
                            WriteIndented = true,
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,

                        }).Replace("\"", ""));

                    var canConnect = context.Database.CanConnect();
                    LoggerManager.Logger.Information($"Db connection {(canConnect ? "successfull" : "failed")}");

                    var created = context.Database.EnsureCreated();
                    LoggerManager.Logger.Information($"Db {(created ? "is recreated" : "already exists")}");
                }
                catch (Exception ex)
                {
                    LoggerManager.Logger.Information($"Db: {context.Database.GetConnectionString()}");
                    LoggerManager.Logger.Error("SQL connection issue appeared: {0}, {1}", ex.Message, ex.InnerException?.Message);
                }
            });
    }
}
