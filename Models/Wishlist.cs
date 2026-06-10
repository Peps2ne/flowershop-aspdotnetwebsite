namespace FlowerShopApp.Models
{
    public class Wishlist
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public List<WishlistItem> Items { get; set; } = new();
    }

    public class WishlistItem
    {
        public int Id { get; set; }
        public int WishlistId { get; set; }
        public Wishlist Wishlist { get; set; } = null!;
        
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        
        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}
