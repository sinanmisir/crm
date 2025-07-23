namespace CRM.DTOs
{
    public class GorevEkleDTO
    {
        public int? MusteriId { get; set; }
        public int? KisiId { get; set; }
        public int? SorumluKullaniciId { get; set; }

        public string? Aciklama { get; set; }
        public DateTime? SonTarih { get; set; }
        public string? Durum { get; set; }
    }
}
