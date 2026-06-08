using ASP_DemoGRPC;
using Grpc.Core;

namespace ASP_DemoGRPC.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;

        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                throw new RpcException(new Status(
                    StatusCode.InvalidArgument,
                    "Name is missing"));
            }

            return Task.FromResult(new HelloReply
            {
                Message = $"Hello {request.Name}!"
            });
        }
    }
}
