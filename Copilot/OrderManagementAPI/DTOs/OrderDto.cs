namespace OrderManagementAPI.DTOs
{
    public class OrderDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
