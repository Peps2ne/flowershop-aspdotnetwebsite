using System.ComponentModel.DataAnnotations;

namespace FlowerShopApp.Models
{
    public class Discount
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "İndirim Kodu zorunludur.")]
        public string Code { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "İndirim Oranı zorunludur.")]
        [Range(1, 100, ErrorMessage = "1 ile 100 arasında bir değer giriniz.")]
        public int DiscountPercentage { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
