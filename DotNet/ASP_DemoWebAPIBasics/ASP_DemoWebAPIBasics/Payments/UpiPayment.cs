namespace ASP_DemoWebAPIBasics.Payments
{
    public class UpiPayment : IPaymentProcessor
    {
        public void ProcessPayment(decimal amount)
        {
            Console.WriteLine($"Processing UPI payment of amount: {amount}");
        }
    }
}
