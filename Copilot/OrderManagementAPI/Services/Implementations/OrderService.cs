using AutoMapper;
using OrderManagementAPI.DTOs;
using OrderManagementAPI.Models;
using OrderManagementAPI.Repositories.Interfaces;
using OrderManagementAPI.Services.Interfaces;

namespace OrderManagementAPI.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        
        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task CreateOrderAsync(OrderDto orderDto)
        {
            if (orderDto == null)
            {
                throw new ArgumentNullException(nameof(orderDto));
            }

            var order = _mapper.Map<Order>(orderDto);
            await _orderRepository.AddOrderAsync(order);
        }

        public async Task<OrderDto?> GetOrderAsync(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            return order == null ? null : _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            //return orders.Select(order => _mapper.Map<OrderDto>(order));
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }
    }
}
