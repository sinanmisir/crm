namespace CRM.DTOs
{
    public class GorevGuncelleDTO
    {
        public int Id { get; set; }
        public int? MusteriId { get; set; }
        public int? KisiId { get; set; }
        public int? SorumluKullaniciId { get; set; }

        public string? MusteriAd { get; set; }  // ✔ Eklendi
        public string? KisiAd { get; set; }     // ✔ Eklendi
        public string? SorumluKullaniciAd { get; set; }  // ✔ Eklendi

        public string? Aciklama { get; set; }
        public DateTime? SonTarih { get; set; }
        public string? Durum { get; set; }
    }
}
