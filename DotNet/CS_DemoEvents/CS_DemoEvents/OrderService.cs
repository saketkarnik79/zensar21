using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoEvents
{
    internal class OrderService
    {
        //public event EventHandler OrderPlaced;
        public event EventHandler<OrderEventArgs> OrderPlaced;

        //public void PlaceOrder(string orderId)
        //{
        //    Console.WriteLine($"Order #{orderId} has been placed.");

        //    // Raise the OrderPlaced event
        //    OrderPlaced?.Invoke(this, EventArgs.Empty);
        //}

        public void PlaceOrder(string orderId, decimal amount)
        {
            Console.WriteLine($"Order #{orderId} has been placed.");

            // Raise the OrderPlaced event with order details
            OrderPlaced?.Invoke(this, new OrderEventArgs { OrderID = orderId, Amount = amount });
        }
    }
}
