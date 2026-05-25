using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderManagementAPI.Models;
using OrderManagementAPI.Services;
using OrderManagementAPI.Services.Interfaces;
using OrderManagementAPI.DTOs;
using MathsLib;

namespace OrderManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> Get()
        {
            Calc calc = new Calc();

            var result = calc.Add(5, 10);

            var orders = await _orderService.GetOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Order>> Get(int id)
        {
            var order = await _orderService.GetOrderAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> Post([FromBody] OrderDto orderDto)
        {
            await _orderService.CreateOrderAsync(orderDto);
            return Created("", null);
        }

        // [HttpPut("{id:int}")]
        // public async Task<IActionResult> Put(int id, [FromBody] Order order)
        // {
        //     if (id != order.Id)
        //     {
        //         return BadRequest();
        //     }

        //     var updatedOrder = await _orderService.UpdateOrderAsync(order);
        //     if (updatedOrder == null)
        //     {
        //         return NotFound();
        //     }

        //     return Ok(updatedOrder);
        // }

        // [HttpDelete("{id:int}")]
        // public async Task<IActionResult> Delete(int id)
        // {
        //     var deleted = await _orderService.DeleteOrderAsync(id);
        //     if (!deleted)
        //     {
        //         return NotFound();
        //     }

        //     return NoContent();
        // }
    }
}
