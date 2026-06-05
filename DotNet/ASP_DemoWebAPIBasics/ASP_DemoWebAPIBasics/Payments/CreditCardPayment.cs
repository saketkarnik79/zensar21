namespace ASP_DemoWebAPIBasics.Payments
{
    public class CreditCardPayment : IPaymentProcessor
    {
        public void ProcessPayment(decimal amount)
        {
            Console.WriteLine($"Processing credit card payment of amount: {amount}");
        }
    }
}
