using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoGoFPatterns.Creational
{
    internal static class PaymentFactory
    {
        public static IPayment CreatePayment(string paymentType)
        {
            return paymentType.ToLower() switch
            {
                "cc" => new CreditCardPayment(),
                "upi" => new UpiPayment(),
                _ => throw new ArgumentException("Invalid payment type")
            };
        }
    }
}
