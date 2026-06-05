namespace ASP_DemoWebAPIBasics.Payments
{
    public interface IPaymentProcessor
    {
        void ProcessPayment(decimal amount);
    }
}
