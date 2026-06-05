using ASP_DemoWebAPIBasics.Models;

namespace ASP_DemoWebAPIBasics.Repositories
{
    public interface IOrderRepository
    {
        void Save(Order order);
    }
}
