using QIA.Plugin.OpcClient.Services;
using QIA.Plugin.OpcClient.Services.Interfaces;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QIA.Plugin.OpcClient.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QIA.Plugin.OpcClient.HostedServices
{
    public class OpcHosted : BackgroundService
    {
        private readonly IAppSettings appSettings;
        private readonly ITreeManager treeManager;
        private readonly SignalRService signalRService;
        public static CancellationTokenSource ShutdownTokenSource = new();
        public static bool success = false;

        public OpcHosted(IAppSettings appSettings, ITreeManager treeManager, SignalRService signalRService)
        {
            this.appSettings = appSettings;
            this.treeManager = treeManager;
            this.signalRService = signalRService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
            {
                var app = OpcConfiguration.GetInstance();
                app.InitSession();

                /// start azure events listening
                //Task.Run(async () => await StartAzureListening(), Program.ShutdownTokenSource.Token);
                /// start the connection
                try
                {
                    await SessionStartAsync();
                    success = true;
                    LoggerManager.Logger.Information("++++Done++++\nPress Ctrl+C to exit");
                    Console.WriteLine("# Subscriptions:");
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    LoggerManager.Logger.Error("500: {0}, {1}", ex.Message + ex.InnerException?.Message, ex);
                    throw;
                }

                /// allow canceling the application
                var quitEvent = new ManualResetEvent(false);
                Console.CancelKeyPress += (sender, eArgs) =>
                {
                    success = true;
                    quitEvent.Set();
                    eArgs.Cancel = true;
                    ShutdownTokenSource.Cancel();
                };

                quitEvent.WaitOne(Timeout.Infinite);
                ShutdownTokenSource.Cancel();
                LoggerManager.Logger.Information("++++EXIT++++");

                LoggerManager.Logger.Information("Stopping opc service...");
            });
        }

        /// <summary>
        /// Start all sessions.
        /// </summary>
        public async Task SessionStartAsync()
        {
            // TODO: how to pass nodes to find property to SubscriptionManager
            /// browse the server tree
            var graphFull = treeManager.BrowseTree();
            await treeManager.WriteTree("graph-full.json", graphFull);

            try
            {
                var result = await treeManager.FindNodesRecursively();
                await treeManager.WriteTree("graph-new.json", result);
            }
            catch (Exception ex)
            {
                LoggerManager.Logger.Error(ex, "Couldn't deserialize nodemanager:");
                throw new Exception("Couldn't deserialize nodemanager", ex.InnerException);
            }

            Console.WriteLine();
            LoggerManager.Logger.Information("# Parsing finished. Graph created.");

            // TODO: init session here and move logic to fictive controller
            // try
            // {
            //     await SessionManager.OpcSessionsListSemaphore.WaitAsync();
            //     SessionManager.OpcSessions.ForEach(s => s.ConnectAndMonitorSession.Set());
            // }
            // catch (Exception e)
            // {
            //     Logger.Fatal(e, "Failed to start all sessions.");
            // }
            // finally
            // {
            //     SessionManager.OpcSessionsListSemaphore.Release();
            // }

            await Task.CompletedTask;
        }

        public async Task StartAzureListening()
        {
            var ams = Extensions.GetService<IAzureMessageService>();
            await ams.ReadMessageAsync();
            await Task.CompletedTask;
        }
    }
}

