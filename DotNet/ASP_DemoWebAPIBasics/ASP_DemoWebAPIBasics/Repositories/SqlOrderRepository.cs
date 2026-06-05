using ASP_DemoWebAPIBasics.Models;
using ASP_DemoWebAPIBasics.DAL;

namespace ASP_DemoWebAPIBasics.Repositories
{
    public class SqlOrderRepository : IOrderRepository
    {
        private readonly DemoWebAPIBasicsDbContext _dbContext;

        public SqlOrderRepository(DemoWebAPIBasicsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Save(Order order)
        {
            _dbContext.Orders.Add(order);
            _dbContext.SaveChanges();
            Console.WriteLine($"Order saved successfully: {order.Id}");
        }
    }
}
