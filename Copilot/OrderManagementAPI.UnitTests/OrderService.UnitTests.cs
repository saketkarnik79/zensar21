using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using OrderManagementAPI.DTOs;
using OrderManagementAPI.Models;
using OrderManagementAPI.Repositories.Interfaces;
using OrderManagementAPI.Services.Implementations;

namespace OrderManagementAPI.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly OrderService _sut;

        public OrderServiceTests()
        {
            _orderRepoMock = new Mock<IOrderRepository>();
            _mapperMock = new Mock<IMapper>();
            _sut = new OrderService(_orderRepoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task CreateOrderAsync_CallsRepositoryAdd_WhenOrderDtoIsValid()
        {
            // Arrange
            var dto = new OrderDto { CustomerName = "Alice", TotalAmount = 123.45m };
            var mappedOrder = new Order { Id = 1, CustomerName = dto.CustomerName, TotalAmount = dto.TotalAmount };

            _mapperMock
                .Setup(m => m.Map<Order>(It.Is<OrderDto>(d => d == dto)))
                .Returns(mappedOrder);

            _orderRepoMock
                .Setup(r => r.AddOrderAsync(It.Is<Order>(o => o == mappedOrder)))
                .Returns(Task.CompletedTask);

            // Act
            await _sut.CreateOrderAsync(dto);

            // Assert
            _mapperMock.Verify(m => m.Map<Order>(dto), Times.Once);
            _orderRepoMock.Verify(r => r.AddOrderAsync(mappedOrder), Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_ThrowsArgumentNullException_WhenOrderDtoIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.CreateOrderAsync(null!));
        }

        [Fact]
        public async Task GetOrderAsync_ReturnsOrderDto_WhenOrderExists()
        {
            // Arrange
            var id = 42;
            var order = new Order { Id = id, CustomerName = "Bob", TotalAmount = 50m };
            var expectedDto = new OrderDto { CustomerName = order.CustomerName, TotalAmount = order.TotalAmount };

            _orderRepoMock
                .Setup(r => r.GetOrderByIdAsync(It.Is<int>(i => i == id)))
                .ReturnsAsync(order);

            _mapperMock
                .Setup(m => m.Map<OrderDto>(It.Is<Order>(o => o == order)))
                .Returns(expectedDto);

            // Act
            var result = await _sut.GetOrderAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDto.CustomerName, result!.CustomerName);
            Assert.Equal(expectedDto.TotalAmount, result.TotalAmount);

            _orderRepoMock.Verify(r => r.GetOrderByIdAsync(id), Times.Once);
            _mapperMock.Verify(m => m.Map<OrderDto>(order), Times.Once);
        }

        [Fact]
        public async Task GetOrderAsync_ReturnsNull_WhenOrderNotFound()
        {
            // Arrange
            var id = 100;
            _orderRepoMock
                .Setup(r => r.GetOrderByIdAsync(It.Is<int>(i => i == id)))
                .ReturnsAsync((Order?)null);

            // Act
            var result = await _sut.GetOrderAsync(id);

            // Assert
            Assert.Null(result);
            _orderRepoMock.Verify(r => r.GetOrderByIdAsync(id), Times.Once);
            _mapperMock.Verify(m => m.Map<OrderDto>(It.IsAny<Order>()), Times.Never);
        }

        [Fact]
        public async Task GetOrdersAsync_ReturnsMappedOrderDtos()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { Id = 1, CustomerName = "A", TotalAmount = 10m },
                new Order { Id = 2, CustomerName = "B", TotalAmount = 20m }
            };

            var mapped = orders.Select(o => new OrderDto { CustomerName = o.CustomerName, TotalAmount = o.TotalAmount }).ToList();

            _orderRepoMock
                .Setup(r => r.GetAllOrdersAsync())
                .ReturnsAsync(orders);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<OrderDto>>(It.IsAny<IEnumerable<Order>>()))
                .Returns((IEnumerable<Order> src) => src.Select(o => new OrderDto { CustomerName = o.CustomerName, TotalAmount = o.TotalAmount }));

            // Act
            var result = await _sut.GetOrdersAsync();

            // Assert
            Assert.NotNull(result);
            var resultList = result.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.Collection(resultList,
                d => { Assert.Equal("A", d.CustomerName); Assert.Equal(10m, d.TotalAmount); },
                d => { Assert.Equal("B", d.CustomerName); Assert.Equal(20m, d.TotalAmount); });

            _orderRepoMock.Verify(r => r.GetAllOrdersAsync(), Times.Once);
            _mapperMock.Verify(m => m.Map<IEnumerable<OrderDto>>(It.IsAny<IEnumerable<Order>>()), Times.Once);
        }
    }
}