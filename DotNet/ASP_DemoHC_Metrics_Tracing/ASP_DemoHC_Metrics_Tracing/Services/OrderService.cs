namespace ASP_DemoHC_Metrics_Tracing.Services
{
    public class OrderService
    {
        public string ProcessOrder(int orderId)
        {
            // Simulate some processing time
            System.Threading.Thread.Sleep(500);
            return $"Order {orderId} has been processed.";
        }
    }
}
