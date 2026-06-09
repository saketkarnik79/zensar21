using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using MassTransit;
using OrderService.Messaging;

namespace OrderService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq://rabbitmq");
                });
            });
            builder.Services.AddScoped<OrderPublisher>();
            builder.Services.AddDbContext<OrderDbContext>(options => options.UseInMemoryDatabase("OrdersDB"));

            builder.Services.AddScoped<HttpClient>();
            //builder.Services.AddCors(options =>
            //{
            //    options.AddDefaultPolicy(policy =>
            //    {
            //        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            //    });
            //});
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            //app.UseAuthorization();

            //app.UseCors();
            app.MapControllers();

            app.Run();
        }
    }
}
