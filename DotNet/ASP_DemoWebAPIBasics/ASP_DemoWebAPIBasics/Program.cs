using Microsoft.EntityFrameworkCore;
using ASP_DemoWebAPIBasics.DAL;
using ASP_DemoWebAPIBasics.Repositories;
using ASP_DemoWebAPIBasics.Payments;
using ASP_DemoWebAPIBasics.Notifications;
using ASP_DemoWebAPIBasics.Services;

namespace ASP_DemoWebAPIBasics
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<DemoWebAPIBasicsDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("cs")));
            builder.Services.AddScoped<IOrderRepository, SqlOrderRepository>();
            builder.Services.AddSingleton<IPaymentProcessor, UpiPayment>();
            builder.Services.AddSingleton<INotifier, EmailNotifier>();

            builder.Services.AddScoped<IOrderService, OrderService>();

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
