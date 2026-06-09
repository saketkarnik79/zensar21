using Ocelot.Middleware;
using Ocelot.DependencyInjection;

namespace ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("OcelotAPIGWConfig.json", optional: false, reloadOnChange: true);

            // Add services to the container.

            builder.Services.AddOcelot(builder.Configuration);
            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseOcelot();

            app.Run();
        }
    }
}
