using System.ComponentModel.DataAnnotations;

namespace FlowerShopApp.Models
{
    public class Review
    {
        public int Id { get; set; }
        
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        [Range(1, 5)]
        public int Rating { get; set; }
        
        [StringLength(1000)]
        public string Comment { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public bool IsApproved { get; set; } = true;
    }
}
