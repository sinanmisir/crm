namespace CRM.DTOs
{
    public class GorevDTO
    {
        public int Id { get; set; }
        public string? MusteriAd { get; set; }
        public string? KisiAd { get; set; }
        public string? SorumluKullaniciAd { get; set; }  // ✔ Bu olmalı
        public string? Aciklama { get; set; }
        public string? Durum { get; set; }
        public DateTime? SonTarih { get; set; }
    }

}
