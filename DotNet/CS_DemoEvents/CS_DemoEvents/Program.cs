namespace CS_DemoEvents
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //    Process process = new Process();

            //    process.ProcessCompleted += OnProcessCompleted; // Subscribe to the event

            //    process.StartProcess(); // Start the process

            OrderService orderService = new OrderService();

            //orderService.OrderPlaced += (sender, e) =>
            //{
            //    Console.WriteLine("Event Received: Order has been placed!");
            //    // InventoryService.UpdateInventory();
            //};

            //orderService.PlaceOrder("12345");
            orderService.OrderPlaced += (sender, e) =>
            {
                Console.WriteLine($"Event Received: Order #{e.OrderID} has been placed with amount ${e.Amount}!");
                // InventoryService.UpdateInventory();
            };

            orderService.PlaceOrder("12345", 250.00m);

            Console.WriteLine("Program completed. Press any key to exit.");
            Console.ReadKey();
        }

        static void OnProcessCompleted()
        {
            Console.WriteLine("Event Received: Process Completed!");
        }
    }
}
