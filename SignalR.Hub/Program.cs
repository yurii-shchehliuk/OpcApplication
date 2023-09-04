using QIA.Plugin.OpcClient.Services;
using WebApplication1.Services;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webClient = "webClient";
            var opcClient = "opcClient";

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddTransient<SignalrService>();
            builder.Services.AddSignalR();
            builder.Services.AddCors(o =>
            {
                o.AddPolicy(webClient, c => c.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins("http://localhost:4200", "127.0.0.1", "localhost"));
                o.AddPolicy(opcClient, c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            });
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors(webClient);
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chatsocket");
            });

            app.Run();
        }
    }
}