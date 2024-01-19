using Microsoft.EntityFrameworkCore;
using Opc.Ua;
using Qia.Opc.Domain.Common;
using Qia.Opc.Infrastructure.Application;
using Qia.Opc.OPCUA.Connector;
using Qia.Opc.OPCUA.Connector.Managers;
using Qia.Opc.Persistence;
using QIA.Opc.API.Core;
using QIA.Opc.Domain.Common;
using QIA.Opc.Domain.Repository;
using QIA.Opc.Infrastructure.Repositories;
using QIA.Opc.Infrastructure.Services.Communication;
using QIA.Opc.Infrastructure.Services.OPCUA;
using QIA.Opc.Infrastructure.ServicesHosted;
using QIA.Opc.OPCUA.Connector.Managers;
using System.Text.Json;

namespace QIA.Opc.API
{
	public class Program
	{
		public async static Task Main(string[] args)
		{
			string webClient = nameof(webClient);
			string anyOrigin = nameof(anyOrigin);

			var builder = WebApplication.CreateBuilder(args);
			LoggerManager.InitLogging();

			// Add services to the container.
			builder.Services.AddControllers().AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			});

			IAppSettings settings = new AppSettings(builder.Configuration);
			builder.Services.AddSingleton<IAppSettings>(settings);

			// app init
			builder.Services.AddSingleton<ApplicationConfiguration>(sp =>
			{
				var configBuilder = new ApplicationConfigBuilder();
				configBuilder.Init().Wait();

				return configBuilder.ApplicationConfiguration;
			});

			// connector
			builder.Services.AddSingleton<SessionManager>();
			builder.Services.AddSingleton<SubscriptionManager>();
			builder.Services.AddScoped<TreeManager>();
			builder.Services.AddTransient<MonitoredItemManager>();

			// infrastructure
			builder.Services.AddTransient<CleanerService>();
			//builder.Services.AddTransient<NodeService>();
			builder.Services.AddScoped<SessionService>();
			builder.Services.AddScoped<TreeService>();
			builder.Services.AddScoped<SubscriptionService>();
			builder.Services.AddScoped<MonitoredItemService>();
			//builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

			// communication
			builder.Services.AddSingleton<SignalRService>();

			// hosted
			builder.Services.AddHostedService<EventHandlerService>();
			//builder.Services.AddHostedService<SessionCleanupService>();

			// database
			builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

			builder.Services.AddDbContextFactory<OpcDbContext>(x =>
												x.UseSqlServer(settings.DbConnectionString));

			// 3rd libs
			builder.Services
				.AddSignalR()
				.AddAzureSignalR(options =>
				{
					options.ConnectionString = "Endpoint=https://dev-qia-opc-api-weu-sr.service.signalr.net;AccessKey=oOkPCAkUWCGJd/sKyYOByLbCSPyQbjssxrl1TIwf7jk=;Version=1.0;";
				});
			builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

			builder.Services.AddMediatR((c) =>
			{
				c.RegisterServicesFromAssembly(typeof(EventMediatorCommand).Assembly);
			});

			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new() { Title = "My API", Version = "v1" });
			});

			// cors
			builder.Services.AddCors(o =>
			{
				o.AddPolicy(webClient, c => c.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:4200", "http://localhost:8086", "https://dev-qia-opc-web-weu-app.azurewebsites.net"));
				o.AddPolicy(anyOrigin, c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
			});

			var app = builder.Build();

			// app run
			app.Services.GetRequiredService<ApplicationConfiguration>();
			await InitDbAsync(app.Services.CreateScope());

			app.UseHttpsRedirection();
			app.UseHsts();

			app.UseMiddleware<ExceptionMiddleware>();

			app.UseCors(webClient);

			app.UseRouting();

			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
			});

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapHub<ChatHub>("/chathub");

				endpoints.MapControllers();
				endpoints.MapControllerRoute(
						name: "default",
						pattern: "{controller}/{action=Index}/{id?}");
			});

			LoggerManager.Logger.Information($"Application started: {builder.Configuration[$"profiles:{app.Environment.EnvironmentName}:applicationUrl"]}");

			app.Run();
		}

		private static async Task InitDbAsync(IServiceScope scope)
		{
			using var context = scope.ServiceProvider.GetRequiredService<OpcDbContext>();
			try
			{
				var created = await context.Database.EnsureCreatedAsync();
				LoggerManager.Logger.Information($"Db {(created ? "is created" : "already exists")}");

				var canConnect = await context.Database.CanConnectAsync();
				LoggerManager.Logger.Information($"Db connection {(canConnect ? "successfull" : "failed")}");
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Information($"Db: {context.Database.GetConnectionString()}");
				LoggerManager.Logger.Error("SQL connection issue appeared: {0}, {1}", ex.Message, ex.InnerException?.Message);
			}
		}
	}
}