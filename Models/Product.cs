using System.ComponentModel.DataAnnotations;

namespace FlowerShopApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string Slug { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public decimal Price { get; set; }
        
        public string ImageUrl { get; set; } = string.Empty;
        
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        
        public int Stock { get; set; } = 10;
        
        public string Badge { get; set; } = string.Empty;
        
        public string BadgeColor { get; set; } = string.Empty;
    }
}
