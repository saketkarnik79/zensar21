namespace CS_DemoAnonymousMethods
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Anonymous method for discount
            //OrderAction applyDiscount = delegate (decimal amount)
            //{
            //    decimal discountedAmount = amount * 0.9m; // 10% discount
            //    Console.WriteLine($"Original Amount: {amount:C}, Discounted Amount: {discountedAmount:C}");
            //};
            OrderAction applyDiscount = (amount) => // Lambda expression for discount
            {
                decimal discountedAmount = amount * 0.9m; // 10% discount
                Console.WriteLine($"Original Amount: {amount:C}, Discounted Amount: {discountedAmount:C}");
            };

            // Anonymous method for logging
            //OrderAction logOrder = delegate (decimal amount)
            //{
            //    Console.WriteLine($"Order placed with amount: {amount:C}");
            //};

            OrderAction logOrder = (amount) => // Lambda expression for logging // Expression-bodied method for logging
                Console.WriteLine($"Order placed with amount: {amount:C}");

            // Anonymous method for notification
            OrderAction notifyCustomer = delegate (decimal amount)
            {
                Console.WriteLine($"Customer notified about order of amount: {amount:C}");
            };

            // Simulate placing an order
            decimal orderAmount = 1000m;
            applyDiscount?.Invoke(orderAmount);
            logOrder?.Invoke(orderAmount);
            notifyCustomer?.Invoke(orderAmount);

            Console.WriteLine("Order processing completed. Press any key to exit...");
            Console.ReadKey();
        }
    }
}
