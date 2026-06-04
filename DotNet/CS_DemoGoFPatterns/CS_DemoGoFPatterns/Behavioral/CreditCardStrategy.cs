using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoGoFPatterns.Behavioral
{
    internal class CreditCardStrategy: IPaymentStrategy
    {
        public void Pay(decimal amount)
        {
            Console.WriteLine($"Processing credit card payment of ${amount:F2}");
        }
    }
}
