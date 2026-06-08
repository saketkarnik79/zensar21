using Grpc.Core;
using Grpc.Net.Client;

namespace GrpcClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using (var channel=GrpcChannel.ForAddress("https://localhost:7128"))
            {
                var client=new Greeter.GreeterClient(channel);

                try
                {
                    var reply = await client.SayHelloAsync(new HelloRequest() { Name = "James" });
                    Console.WriteLine(reply.Message);
                }
                catch (RpcException ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }

            Console.WriteLine("Program completed. Press any key to exit...");
            Console.ReadKey();

        }
    }
}
