using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoGoFPatterns.Behavioral
{
    internal class PaymentContext
    {
        private IPaymentStrategy strategy;

        public PaymentContext(IPaymentStrategy strategy)
        {
            this.strategy = strategy;
        }

        public void SetStrategy(IPaymentStrategy strategy)
        {
            this.strategy = strategy;
        }

        public void Pay(decimal amount)
        {
            strategy.Pay(amount);
        }
    }
}
