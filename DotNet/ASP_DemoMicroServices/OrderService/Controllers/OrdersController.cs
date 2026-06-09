using Grpc.Net.Client;
using InventoryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Text;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Create()
        {
            var channel = GrpcChannel.ForAddress("http://localhost:5121");
            var client = new Inventory.InventoryClient(channel);
            var result = await client.CheckStockAsync(new StockRequest() { ProductId = 1 });

            if (!result.InStock)
            {
                return BadRequest("Out of stock.");
            }
            await PublishEvent();
            return Ok("Order created successfully.");
        }

        private async Task PublishEvent()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = await factory.CreateConnectionAsync())
            {
                using (var channel = await connection.CreateChannelAsync()) 
                {
                    await channel.QueueDeclareAsync("ordersq", false, false, false);
                    var body = Encoding.UTF8.GetBytes("OrderCreated");
                    await channel.BasicPublishAsync("", "ordersq", false, body);
                }
            }
        }
    }
}
