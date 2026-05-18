using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderManagementAPI.DTOs;

namespace OrderManagementAPI.Services.Interfaces
{
    /// <summary>
    /// Public contract for order operations exposed by the service layer.
    /// </summary>
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetOrdersAsync();

        Task<OrderDto?> GetOrderAsync(int id);

        Task CreateOrderAsync(OrderDto orderDto);

        // Task<bool> UpdateOrderAsync(Guid id, UpdateOrderDto updateDto);

        // Task<bool> CancelOrderAsync(Guid id);
    }
}
