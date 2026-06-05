using ASP_DemoWebAPIBasics.Models;

namespace ASP_DemoWebAPIBasics.Services
{
    public interface IOrderService
    {
        void PlaceOrder(Order order);
    }
}
