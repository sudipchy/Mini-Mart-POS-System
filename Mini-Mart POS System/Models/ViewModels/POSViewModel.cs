namespace MiniMartPOS.Models.ViewModels
{
    public class POSViewModel
    {
        public List<CartItem> CartItems { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal ChangeAmount { get; set; }
        public string? PaymentMethod { get; set; }
        public int? CustomerId { get; set; }
        public string? Notes { get; set; }
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
    }

    public class SaleRequest
    {
        public List<CartItem> CartItems { get; set; } = new();
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal PaidAmount { get; set; }
        public string? PaymentMethod { get; set; }
        public int? CustomerId { get; set; }
        public string? Notes { get; set; }
    }
}
