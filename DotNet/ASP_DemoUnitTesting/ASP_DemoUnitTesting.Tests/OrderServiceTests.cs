using ASP_DemoUnitTesting.Services;
using Moq;

namespace ASP_DemoUnitTesting.Tests
{
    public class OrderServiceTests: IDisposable
    {
        public OrderServiceTests() // Runs always before each test
        {
            
        }

        public void Dispose() // Runs always after each test
        {
            // Clean Up code
        }

        [Theory]
        //[InlineData(100, true)]
        [InlineData(0, false)]
        [InlineData(-100, false)]
        //[InlineData(0.01, true)]
        public void IsValidAmount_ReturnsExpected(decimal amount, bool expected)
        {
            // Arrange
            //var orderService = new OrderService();
            var orderServiceMock= new Mock<IOrderService>();
            var orderService = orderServiceMock.Object;

            // Act
            var result = orderService.IsValidAmount(amount);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}