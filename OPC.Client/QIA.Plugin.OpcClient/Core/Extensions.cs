using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QIA.Library.Worker;
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
        private static ServiceProvider serviceProvider;

        public static void InjectProvider(IServiceCollection serviceCollection)
        {
            serviceProvider = serviceCollection.BuildServiceProvider();
        }
        public static string ReadServerUrl() => ReadSettings().OpcUrl;
        public static IAppSettings ReadSettings() => serviceProvider.GetRequiredService<IOptions<IAppSettings>>().Value;

        public static IAppSettings ReadSettingsDynamic(IServiceProvider serviceProvider)  =>
            serviceProvider.GetRequiredService<IOptionsMonitor<IAppSettings>>().CurrentValue;

        public static T GetService<T>() where T : class => serviceProvider.GetRequiredService<T>();
    }
}
