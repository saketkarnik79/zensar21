using CS_DemoGoFPatterns.Creational;
using CS_DemoGoFPatterns.Structural;
using CS_DemoGoFPatterns.Behavioral;

namespace CS_DemoGoFPatterns
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Demo for Factory Method Pattern
            //IPayment payment = PaymentFactory.CreatePayment("upi");
            //payment.Process();

            //Demo for Singleton Pattern
            //Logger logger1 = Logger.Instance;
            //Logger logger2 = Logger.Instance;
            //logger1.Log("This is a log message from logger1.");
            //logger2.Log("This is a log message from logger2.");
            //Logger.Instance.Log("This is a log message from the singleton logger.");
            //Logger.Instance.Log("This is another log message from the singleton logger.");

            // Demo for Repository Pattern
            //IEmployeeRepository employeeRepository = new EmployeeRepository();
            //EmployeeService employeeService = new EmployeeService(employeeRepository);

            //employeeService.AddEmployee("John Doe");
            //employeeService.AddEmployee("Jane Smith");
            //employeeService.PrintAll();

            // Demo for Decorator pattern
            //INotifier notifier = new EmailNotifier();
            //notifier.Send("Payment processed...");

            //INotifier notifier2 = new EmailNotifier();
            //notifier2 = new SmsNotifier(notifier2);
            //notifier2 = new SlackNotifier(notifier2);

            //notifier2.Send("Payment processed with multiple notifications...");

            // Demo for Strategy pattern
            //var paymentContext = new PaymentContext(new CreditCardStrategy());
            //paymentContext.Pay(100.00m);
            //paymentContext.SetStrategy(new UpiStrategy());
            //paymentContext.Pay(200.00m);

            Channel channel = new Channel();
            var user1 = new Subscriber("Alice");
            var user2 = new Subscriber("Bob");
            channel.Subscribe(user1);
            channel.Subscribe(user2);

            channel.UploadVideo("Design Patterns in C# - Observer Pattern Demo");

            Console.WriteLine("Payment processed. Press any key to exit...");
            Console.ReadKey();
        }
    }
}
