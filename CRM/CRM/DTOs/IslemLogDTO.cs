namespace CRM.DTOs
{
    public class IslemLogDTO
    {
        public string KullaniciAd { get; set; } = null!;
        public string? Islem { get; set; }
        public DateTime Tarih { get; set; }
    }
}
