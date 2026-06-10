using System.ComponentModel.DataAnnotations;

namespace FlowerShopApp.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        
        public int ShoppingCartId { get; set; }
        public ShoppingCart ShoppingCart { get; set; } = null!;
        
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        
        public int Quantity { get; set; } = 1;
    }
}
