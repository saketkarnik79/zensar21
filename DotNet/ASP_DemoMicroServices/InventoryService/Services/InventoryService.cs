using Grpc.Core;
using InventoryService;

namespace InventoryService.Services
{
    public class InventoryService : Inventory.InventoryBase
    {
        private readonly ILogger<InventoryService> _logger;
        public InventoryService(ILogger<InventoryService> logger)
        {
            _logger = logger;
        }

        public override Task<StockReply> CheckStock(StockRequest request, ServerCallContext context)
        {
            return Task.FromResult(new StockReply
            {
                InStock = true
            });
        }
    }
}
