using System.ComponentModel.DataAnnotations;

namespace FlowerShopApp.Models
{
    public class Address
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        [Required(ErrorMessage = "Adres başlığı zorunludur")]
        [StringLength(50)]
        public string Title { get; set; } = string.Empty; // Ev, İş, vb.
        
        [Required(ErrorMessage = "Ad Soyad alanı zorunludur")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Telefon numarası zorunludur")]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Şehir zorunludur")]
        [StringLength(50)]
        public string City { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Açık adres zorunludur")]
        [StringLength(500)]
        public string FullAddress { get; set; } = string.Empty;
    }
}
