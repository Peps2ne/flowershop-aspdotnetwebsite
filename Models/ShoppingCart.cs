using System.ComponentModel.DataAnnotations;

namespace FlowerShopApp.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        
        public int? UserId { get; set; }
        public User? User { get; set; }
        
        public string SessionId { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public List<ShoppingCartItem> Items { get; set; } = new();
    }
}
