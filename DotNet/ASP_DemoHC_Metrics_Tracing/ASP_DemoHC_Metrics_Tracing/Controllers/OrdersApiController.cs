using ASP_DemoHC_Metrics_Tracing.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_DemoHC_Metrics_Tracing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersApiController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersApiController()
        {
            _orderService = new OrderService();
        }

        //[HttpGet]
        //[Route("{id}")]
        [HttpGet("{id}")]
        public IActionResult Get(int id) 
        {
            var result = _orderService.ProcessOrder(id);
            return Ok(result);
        }
    }
}
