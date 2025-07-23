namespace CRM.DTOs
{
    public class NotEkleDTO
    {
        public int? MusteriId { get; set; }
        public int? KisiId { get; set; } // isteğe bağlı
        public string? Baslik { get; set; }
        public string? Icerik { get; set; }
        public DateTime? Tarih { get; set; }
    }
}
