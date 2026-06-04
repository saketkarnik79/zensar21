using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoGoFPatterns.Creational
{
    internal class CreditCardPayment : IPayment
    {
        public void Process()
        {
            // Credit card payment processing logic
            Console.WriteLine("Processing credit card payment...");
        }
    }
}
