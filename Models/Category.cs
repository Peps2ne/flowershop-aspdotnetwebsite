using System.ComponentModel.DataAnnotations;

namespace FlowerShopApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        public string Slug { get; set; } = string.Empty;
        
        public string Icon { get; set; } = string.Empty;
        
        public List<Product> Products { get; set; } = new();
    }
}
