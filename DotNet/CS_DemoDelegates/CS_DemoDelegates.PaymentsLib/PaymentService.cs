using System;
using System.Collections.Generic;
using System.Text;

namespace CS_DemoDelegates.PaymentsLib
{
    public class PaymentService
    {
        public void CreditCardPayment(decimal amount)
        {
            Console.WriteLine($"Processing payment of {amount:C} using credit card.");
        }

        public void PayPalPayment(decimal amount)
        {
            Console.WriteLine($"Processing payment of {amount:C} using PayPal.");
        }

        public void UpiPayment(decimal amount)
        {
            Console.WriteLine($"Processing payment of {amount:C} using UPI.");
        }

        public void NetBankingPayment(decimal amount)
        {
            Console.WriteLine($"Processing payment of {amount:C} using net banking.");
        }
    }
}
