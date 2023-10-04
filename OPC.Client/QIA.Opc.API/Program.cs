using Microsoft.EntityFrameworkCore;
using Opc.Ua;
using Qia.Opc.Communication;
using Qia.Opc.Domain.Common;
using Qia.Opc.Domain.Core;
using Qia.Opc.Infrastrucutre.Services.Communication;
using Qia.Opc.Infrastrucutre.Services.OPCUA;
using Qia.Opc.OPCUA.Connector;
using Qia.Opc.OPCUA.Connector.Managers;
using Qia.Opc.OPCUA.Connector.ServicesHosted;
using Qia.Opc.Persistence;
using Qia.Opc.Persistence.Repository;
using System.Text.Json;

namespace QIA.Opc.API
{
    public class Program
	{
		public async static Task Main(string[] args)
		{
			var webClient = "webClient";
			var anyOrigin = "opcClient";

			var builder = WebApplication.CreateBuilder(args);
			LoggerManager.InitLogging();
			// Add services to the container.
			builder.Services.AddControllers();
			//.AddJsonOptions(options =>
			// {
			//	 options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			// });

			var configBuilder = new ApplicationConfigBuilder();
			configBuilder.Init().Wait();
			builder.Services.AddSingleton<ApplicationConfiguration>(configBuilder.ApplicationConfiguration);

			IAppSettings settings = new AppSettings(builder.Configuration);
			builder.Services.AddSingleton<IAppSettings>(settings);

			// connector
			builder.Services.AddSingleton<SessionManager>();
			builder.Services.AddSingleton<SubscriptionManager>();
			builder.Services.AddScoped<MonitoredItemManager>();
			builder.Services.AddScoped<TreeManager>();
			builder.Services.AddTransient<NodeManager>();

			// infrastructure
			builder.Services.AddTransient<CleanerService>();
			builder.Services.AddScoped<NodeService>();
			builder.Services.AddScoped<SessionService>();
			builder.Services.AddScoped<TreeService>();
			builder.Services.AddScoped<SubscriptionService>();

			// communication
			builder.Services.AddSingleton<SignalRService>();

			// hosted
			//builder.Services.AddHostedService<SessionCleanupService>();
			builder.Services.AddHostedService<SignalrHosted>();

			// database
			builder.Services.AddScoped(typeof(IDataRepository<>), typeof(DataRepository<>));

			builder.Services.AddDbContextFactory<OpcDbContext>(x =>
												x.UseSqlServer(settings.DbConnectionString));

			// 3rd libs
			builder.Services.AddSignalR();
			builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new() { Title = "My API", Version = "v1" });
			});

			// cors
			builder.Services.AddCors(o =>
			{
				o.AddPolicy(webClient, c => c.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:4200", "127.0.0.1", "localhost"));
				o.AddPolicy(anyOrigin, c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
			});

			var app = builder.Build();

			var scope = app.Services.CreateScope();
			await InitDb(scope);

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseMiddleware<ExceptionMiddleware>();
			app.UseCors(webClient);
			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
			});

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();


				endpoints.MapControllerRoute(
						name: "default",
						pattern: "{controller}/{action=Index}/{id?}");
			});

			app.Run();
		}

		private static async Task InitDb(IServiceScope scope)
		{
			using var context = scope.ServiceProvider.GetRequiredService<OpcDbContext>();
			try
			{
				var canConnect = await context.Database.CanConnectAsync();
				LoggerManager.Logger.Information($"Db connection {(canConnect ? "successfull" : "failed")}");

				var created = await context.Database.EnsureCreatedAsync();
				LoggerManager.Logger.Information($"Db {(created ? "is created" : "already exists")}");
			}
			catch (Exception ex)
			{
				LoggerManager.Logger.Information($"Db: {context.Database.GetConnectionString()}");
				LoggerManager.Logger.Error("SQL connection issue appeared: {0}, {1}", ex.Message, ex.InnerException?.Message);
			}
		}
	}
}