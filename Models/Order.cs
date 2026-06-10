using System.ComponentModel.DataAnnotations;

namespace FlowerShopApp.Models
{
    public enum OrderStatus
    {
        Pending,        // Hazırlanıyor
        Shipped,        // Yolda
        Delivered,      // Teslim Edildi
        Cancelled       // İptal Edildi
    }

    public class Order
    {
        public int Id { get; set; }
        
        public string UserId { get; set; } = string.Empty;
        
        public DateTime OrderDate { get; set; } = DateTime.Now;
        
        public decimal TotalAmount { get; set; }
        
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        
        [Required]
        public string ShippingAddress { get; set; } = string.Empty;
        
        // Simulating payment info
        public string CardLast4 { get; set; } = string.Empty;
        
        public List<OrderItem> Items { get; set; } = new();
    }
}
