namespace CRM.DTOs
{
    public class MusteriDTO
    {
        public int Id { get; set; }
        public string? AdSoyad { get; set; }
        public string? Telefon { get; set; }
        public string? Eposta { get; set; }

        public List<GorevDTO>? Gorevler { get; set; }
        public List<NotDTO>? Notlar { get; set; }
        public List<FirsatDTO>? Firsatlar { get; set; }
        public List<KisiDTO>? Kisiler { get; set; }
        public List<string>? Etiketler { get; set; }

        public List<IslemLogDTO>? IslemLoglari { get; set; } // ✅ Yeni eklendi
    }
}
