using Microsoft.Extensions.Configuration;
using QIA.Library.Worker;
using QIA.Plugin.OpcClient.Core;
using QIA.Plugin.OpcClient.Services.Interfaces;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace QIA.Plugin.OpcClient
{
    /// <summary>
    /// Opc.Worker configDir=C:\dev\QIA_Service\ProjectRollout\OPC_UA\bin-folder\config appSettings=Databases,Assemblies,Logging,Logins,Workers.OPC_UA appControl=appControl
    /// </summary>
    class Program : QIAWorker
    {
        public static CancellationTokenSource ShutdownTokenSource = new();
        public static bool success = false;
        public Program(IConfiguration config, string section) : base(config, section)
        {
        }

        static void Main()
        {
            MainInit();
        }
        protected override bool Init()
        {
            MainInit();
            return success;
        }

        public override string QIAClassSignature => "..o?/\u0010?\u007f??5?]\u001f\n???\u000f?? ??\u007f??b?\n?\v";

        private static void MainInit()
        {
            /// configuration
            Extensions.InjectServices();

            var app = AppConfiguration.GetInstance();
            app.InitSession();

            /// start azure events listening
            //Task.Run(async () => await StartAzureListening(), Program.ShutdownTokenSource.Token);
            /// start the connection
            Task.Run(async () => await SessionStartAsync(), ShutdownTokenSource.Token);

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
        }

        /// <summary>
        /// Start all sessions.
        /// </summary>
        public async static Task SessionStartAsync()
        {
            // TODO: how to pass nodes to find property to SubscriptionManager
            var nm = Extensions.GetService<ITreeManager>();
            /// browse the server tree
            var graphFull = nm.BrowseTree();
            File.WriteAllText("graph-full.json", JsonSerializer.Serialize(graphFull));

            // TODO: search by parsed tree to improve perofrmance
            // TreeSearch<SampleNodeDTO> search = new();
            // search.SearchAndBuild(res1, nodesToFind);

            var result = nm.FindNodesRecursively();
            File.WriteAllText("graph-new.json", JsonSerializer.Serialize(result));

            Console.WriteLine();
            LoggerManager.Logger.Information("Parsing finished. Graph created.");

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


        public async static Task StartAzureListening()
        {
            var ams = Extensions.GetService<IAzureMessageService>();
            await ams.ReadMessageAsync();
            await Task.CompletedTask;
        }
    }
}
