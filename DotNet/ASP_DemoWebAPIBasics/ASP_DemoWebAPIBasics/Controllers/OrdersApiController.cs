using ASP_DemoWebAPIBasics.Models;
using ASP_DemoWebAPIBasics.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASP_DemoWebAPIBasics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersApiController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersApiController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public IActionResult PlaceOrder([FromBody] Order order)
        {
            try
            {
                if(ModelState.IsValid) {
                    _orderService.PlaceOrder(order);
                    return Ok(new { Message = "Order placed successfully." });
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return BadRequest(ModelState);
            }
        }
    }
}
