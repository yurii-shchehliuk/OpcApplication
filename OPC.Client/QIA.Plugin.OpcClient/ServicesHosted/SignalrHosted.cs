using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using QIA.Plugin.OpcClient;
using QIA.Plugin.OpcClient.Core;
using QIA.Plugin.OpcClient.Entities;
using QIA.Plugin.OpcClient.Services;
using QIA.Plugin.OpcClient.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace QIA.Plugin.OpcClient.HostedServices
{
    /// <summary>
    /// message listener to modify json
    /// </summary>
    public class SignalrHosted : BackgroundService
    {
        private readonly SignalRService signalRService;

        public SignalrHosted(SignalRService signalRService)
        {
            this.signalRService = signalRService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
            {
                var result = await signalRService.StartConnectionAsync();
                if (!result)
                {
                    return;
                }
                await signalRService.SendGroupsAction();

                signalRService.ListenGetGroups();
                signalRService.ListenAddGroup();
                signalRService.ListenRemoveGroup();

                signalRService.ListenGetSettings();
                signalRService.ListenSaveSettings();

                signalRService.ListenGetGraph();

                signalRService.ListenNodeMonitor();
                signalRService.ListenGetNodes();
                //TODO: join particular group
                await signalRService.JoinGroup("Group1");

                var exitEvent = new ManualResetEventSlim();
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    eventArgs.Cancel = true;
                    exitEvent.Set();
                };

                // Wait until Ctrl+C is pressed
                exitEvent.Wait();

                await signalRService.StopConnection();
            });
        }
    }
}
