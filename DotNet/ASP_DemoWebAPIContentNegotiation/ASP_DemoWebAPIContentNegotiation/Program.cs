
namespace ASP_DemoWebAPIContentNegotiation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //builder.Services.AddControllers(); // Add support for controllers and API endpoints without content negotiation.
            //builder.Services.AddControllers().AddXmlSerializerFormatters(); // Add support for XML content negotiation.
            builder.Services.AddControllers(options => options.RespectBrowserAcceptHeader = true // Enable content negotiation based on the Accept header along with support for browser accept headers.
            ).AddXmlSerializerFormatters(); // Add support for XML content negotiation.


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

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
