namespace CRM.DTOs
{
    public class NotGuncelleDTO
    {
        public int Id { get; set; }
        public int? MusteriId { get; set; }
        public int? KisiId { get; set; }
        public string? Baslik { get; set; }
        public string? Icerik { get; set; }
        public DateTime? Tarih { get; set; }
    }
}
