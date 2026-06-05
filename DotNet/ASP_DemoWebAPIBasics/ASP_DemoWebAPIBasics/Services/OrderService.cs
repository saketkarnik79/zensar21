using ASP_DemoWebAPIBasics.Models;
using ASP_DemoWebAPIBasics.Notifications;
using ASP_DemoWebAPIBasics.Payments;
using ASP_DemoWebAPIBasics.Repositories;

namespace ASP_DemoWebAPIBasics.Services
{
    public class OrderService : IOrderService
    {
        IOrderRepository _orderRepository;
        IPaymentProcessor _paymentProcessor;
        INotifier _notifier;

        public OrderService(IOrderRepository orderRepository, IPaymentProcessor paymentProcessor, INotifier notifier)
        {
            _orderRepository = orderRepository;
            _paymentProcessor = paymentProcessor;
            _notifier = notifier;
        }

        public void PlaceOrder(Order order)
        {
            _paymentProcessor.ProcessPayment(order.Amount);
            _orderRepository.Save(order);
            _notifier.Send("Your order has been placed successfully.");
        }
    }
}
