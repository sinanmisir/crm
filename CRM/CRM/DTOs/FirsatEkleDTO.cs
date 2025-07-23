using CRM.Models;

namespace CRM.DTOs
{
    public class FirsatEkleDTO
    {
        public int? MusteriId { get; set; }
        public string? Baslik { get; set; }
        public FirsatAsama? Asama { get; set; }

        public decimal? TahminiGelir { get; set; }
        public DateTime? Tarih { get; set; }
    }
}
