namespace CRM.DTOs
{
    public class MusteriEkleDTO
    {
        public string? AdSoyad { get; set; }
        public string? Telefon { get; set; }
        public string? Eposta { get; set; }
        public string? Adres { get; set; }
        public int? KullaniciId { get; set; }

        public List<int> SecilenEtiketler { get; set; } = new();
    }
}
