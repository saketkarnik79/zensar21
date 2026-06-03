using System.Threading.Tasks;

namespace CS_DemoTPL
{
    internal class Program
    {
        static async Task<string> GetUserAsync()
        {
            Console.WriteLine("Fetching user information...");
            await Task.Delay(10000); // Simulate a long-running operation
            return "John Doe";
        }

        static async Task<string[]> GetOrdersAsync()
        {
            Console.WriteLine("Fetching orders...");
            await Task.Delay(5000); // Simulate a long-running operation
            return new string[] { "Order1", "Order2", "Order3" };
        }

        static async Task DoWorkAsync(CancellationToken token)
        {
            Console.WriteLine("Work started...");
            // Simulate work that can be cancelled. Cancel this task after 10 seconds.
            for(int i = 0; i < 10; i++)
            {
                //if (token.IsCancellationRequested)
                //{
                //    Console.WriteLine("Work cancelled.");
                //    return;
                //}
                token.ThrowIfCancellationRequested(); // This will throw an OperationCanceledException if cancellation has been requested
                await Task.Delay(1000, token); // Delay with cancellation support
                Console.WriteLine($"Step {i + 1} completed.");
            }
            Console.WriteLine("Work completed.");
        }

        static async Task Main(string[] args)
        {
            //Console.WriteLine("Application started...");

            //// Run async tasks concurrently
            //var userTask = GetUserAsync();
            //var ordersTask = GetOrdersAsync();

            //// Await both tasks to complete
            //await Task.WhenAll(userTask, ordersTask);

            //var user = await userTask;
            //var orders = await ordersTask;

            //Console.WriteLine($"User: {user}");
            //Console.WriteLine("Orders:");
            //foreach (var order in orders)
            //{
            //    Console.WriteLine($" - {order}");
            //}

            // Create a cancellation token source that cancels after 5 seconds
            var cts = new CancellationTokenSource(5000);
            //cts.Cancel();

            // Run the async task with cancellation support
            try
            {
                // Start the work and pass the cancellation token
                await DoWorkAsync(cts.Token);
                Console.WriteLine("Work completed successfully.");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Work was cancelled due to timeout.");
            }

            Console.WriteLine("Application completed. Press any key to exit...");
            Console.ReadKey();
        }
    }
}
