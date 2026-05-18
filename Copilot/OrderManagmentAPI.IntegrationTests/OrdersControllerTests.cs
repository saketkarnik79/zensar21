using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagementAPI;
using OrderManagementAPI.Data;
using OrderManagementAPI.DTOs;
using Xunit;

namespace OrderManagmentAPI.IntegrationTests
{
    public class OrdersControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public OrdersControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private WebApplicationFactory<Program> CreateFactoryWithInMemoryDb(string dbName)
        {
            return _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace existing AppDbContext registration with a test-specific InMemory DB
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(dbName);
                    });

                    // Ensure DB is created
                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                });
            });
        }

        [Fact]
        public async Task Get_ReturnsEmptyList_WhenNoOrdersExist()
        {
            // Arrange
            var factory = CreateFactoryWithInMemoryDb(Guid.NewGuid().ToString());
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/orders");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<IEnumerable<OrderDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(orders);
            Assert.Empty(orders);
        }

        [Fact]
        public async Task Post_CreatesOrder_ThenGetReturnsCreatedOrder()
        {
            // Arrange
            var factory = CreateFactoryWithInMemoryDb(Guid.NewGuid().ToString());
            var client = factory.CreateClient();

            var newOrder = new OrderDto
            {
                CustomerName = "Integration Test User",
                TotalAmount = 123.45m
            };

            var payload = JsonSerializer.Serialize(newOrder);
            var content = new StringContent(payload, Encoding.UTF8, "application/json");

            // Act - create
            var postResponse = await client.PostAsync("/api/orders", content);

            // Assert create response
            Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);

            // Act - fetch all
            var getResponse = await client.GetAsync("/api/orders");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var json = await getResponse.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<List<OrderDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert that the created order appears in the listing
            Assert.NotNull(orders);
            var created = orders!.FirstOrDefault(o => o.CustomerName == newOrder.CustomerName && o.TotalAmount == newOrder.TotalAmount);
            Assert.NotNull(created);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_ForMissingOrder()
        {
            // Arrange
            var factory = CreateFactoryWithInMemoryDb(Guid.NewGuid().ToString());
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/orders/9999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Post_ReturnsBadRequest_WhenBodyIsEmpty()
        {
            // Arrange
            var factory = CreateFactoryWithInMemoryDb(Guid.NewGuid().ToString());
            var client = factory.CreateClient();

            // Empty body
            var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/api/orders", content);

            // Assert
            // With [ApiController], an empty body for a complex type typically results in 400 Bad Request.
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}