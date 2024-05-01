namespace QIA.Opc.API;

using System.Text.Json;
using global::Opc.Ua;
using Microsoft.EntityFrameworkCore;
using Qia.Opc.OPCUA.Connector;
using QIA.Opc.API.Middleware;
using QIA.Opc.Application.Settings;
using QIA.Opc.Domain.Repositories;
using QIA.Opc.Infrastructure;
using QIA.Opc.Infrastructure.Application;
using QIA.Opc.Infrastructure.DataAccess;
using QIA.Opc.Infrastructure.Managers;
using QIA.Opc.Infrastructure.Repositories;
using QIA.Opc.Infrastructure.Services;
using QIA.Opc.Infrastructure.Services.Communication;
using QIA.Opc.Infrastructure.Services.OPCUA;

public class Program
{
    public static async Task Main(string[] args)
    {
        string webClient = nameof(webClient);
        string anyOrigin = nameof(anyOrigin);

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        LoggerManager.InitLogging();

        // Add services to the container.
        builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

        IAppSettings settings = new AppSettings(builder.Configuration);
        builder.Services.AddSingleton(settings);

        // app init
        builder.Services.AddSingleton(sp =>
        {
            ApplicationConfigBuilder configBuilder = new();
            configBuilder.Init().Wait();

            return configBuilder.ApplicationConfiguration;
        });

        // connector
        builder.Services.AddSingleton<SessionManager>();
        builder.Services.AddSingleton<SubscriptionManager>();
        builder.Services.AddScoped<TreeManager>();
        builder.Services.AddTransient<MonitoredItemManager>();

        // infrastructure
        builder.Services.AddScoped<SessionService>();
        builder.Services.AddScoped<TreeService>();
        builder.Services.AddScoped<SubscriptionService>();
        builder.Services.AddScoped<MonitoredItemService>();
        builder.Services.AddScoped<QueueService>();
        builder.Services.AddSingleton(typeof(UniqueQueue<>));

        // communication
        builder.Services.AddSingleton<SignalRService>();
        builder.Services.AddScoped<AzureMessageService>();

        // database
        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        builder.Services.AddDbContextFactory<OpcDbContext>(x =>
                                            x.UseSqlServer(settings.DbConnectionString));

        // 3rd libs
        builder.Services
            .AddSignalR()
            .AddAzureSignalR(options => options.ConnectionString = settings.AzureSignalR);

        builder.Services.AddAutoMapper(AssemblyReference.Assembly);
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "My API", Version = "v1" }));

        // cors
        builder.Services.AddCors(o =>
        {
            o.AddPolicy(webClient, c => c.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(settings.CORS));
            o.AddPolicy(anyOrigin, c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
        });

        WebApplication app = builder.Build();

        // app run
        app.Services.GetRequiredService<ApplicationConfiguration>();

        using (IServiceScope scope = app.Services.CreateScope())
        {
            await InitDbAsync(scope);
        }

        app.UseHttpsRedirection();
        app.UseHsts();

        app.UseMiddleware<ExceptionMiddleware>();

        app.UseCors(webClient);

        app.UseRouting();

        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));

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
        using OpcDbContext context = scope.ServiceProvider.GetRequiredService<OpcDbContext>();
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
