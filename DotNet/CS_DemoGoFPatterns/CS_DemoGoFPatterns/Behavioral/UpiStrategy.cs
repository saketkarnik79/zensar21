using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoGoFPatterns.Behavioral
{
    internal class UpiStrategy : IPaymentStrategy
    {
        public void Pay(decimal amount)
        {
            Console.WriteLine($"Processing UPI payment of ${amount:F2}");
        }
    }
}
