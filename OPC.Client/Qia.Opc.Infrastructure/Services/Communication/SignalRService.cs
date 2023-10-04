using Microsoft.AspNetCore.SignalR.Client;
using Qia.Opc.Domain.Common;
using Qia.Opc.Domain.Core;

namespace Qia.Opc.Infrastrucutre.Services.Communication
{
    public class SignalRService
	{
		private readonly HubConnection _hubConnection;

		string hubUrl = "";
		public SignalRService(IAppSettings appSettings)
		{
			hubUrl = appSettings.SignalR.HubUrl;

			_hubConnection = new HubConnectionBuilder()
					.WithUrl(hubUrl)
					.Build();

			_hubConnection.Closed += (exception) =>
			{
				var isConnected = StartConnectionAsync().Result;
				return Task.CompletedTask;
			};
		}

		public async Task<bool> StartConnectionAsync()
		{
			try
			{
				if (_hubConnection is null || _hubConnection.State == HubConnectionState.Disconnected)
				{
					await _hubConnection.StartAsync();
				}

				Console.WriteLine(string.Format("# Connected to the {0} Hub", hubUrl));
				return true;
			}
			catch
			{
				LoggerManager.Logger.Error(string.Format("Cannot connect to SignalR: {0}", hubUrl));
				return false;
			}
		}

		#region actions
		public async Task JoinGroup(string sessionId)
		{
			await _hubConnection.InvokeAsync("JoinGroup", sessionId);
		}
		public async Task LeaveGroup(string sessionId)
		{
			await _hubConnection.InvokeAsync("LeaveGroup", sessionId);
		}

		public async Task SendNodeAction(object nodeData, string groupName = "All")
		{
			await _hubConnection.InvokeAsync("SendNodeAction", groupName, nodeData);
		}

		#endregion

		public async Task StopConnection()
		{
			await _hubConnection.StopAsync();
		}
	}
}
