using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using QIA.Plugin.OpcClient;
using QIA.Plugin.OpcClient.Core;
using QIA.Plugin.OpcClient.Core.Settings;
using QIA.Plugin.OpcClient.DTOs;
using QIA.Plugin.OpcClient.Entities;
using QIA.Plugin.OpcClient.Services;
using QIA.Plugin.OpcClient.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SignalR.Client
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
                await signalRService.StartConnectionAsync();

                signalRService.ListenGraphSend();
                signalRService.ListenMessages();
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
