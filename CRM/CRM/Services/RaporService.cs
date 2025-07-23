using CRM.Models;

namespace CRM.Services
{
    public class RaporService
    {
        private readonly CrmDbContext _context;

        public RaporService(CrmDbContext context)
        {
            _context = context;
        }

        public int ToplamMusteriSayisi(int? kullaniciId = null)
        {
            var sorgu = _context.Musterilers.Where(m => m.SilindiMi == false || m.SilindiMi == null);
            if (kullaniciId.HasValue)
                sorgu = sorgu.Where(m => m.KullaniciId == kullaniciId);

            return sorgu.Count();
        }


        public int ToplamGorevSayisi(int? kullaniciId = null)
        {
            var sorgu = _context.Gorevlers.Where(g => g.SilindiMi == false || g.SilindiMi == null);
            if (kullaniciId.HasValue)
                sorgu = sorgu.Where(g => g.SorumluKullaniciId == kullaniciId);

            return sorgu.Count();
        }


        public int TamamlananGorevSayisi(int? kullaniciId = null)
        {
            var sorgu = _context.Gorevlers
                .Where(g => (g.SilindiMi == false || g.SilindiMi == null) && g.Durum == "Tamamlandı");
            if (kullaniciId.HasValue)
                sorgu = sorgu.Where(g => g.SorumluKullaniciId == kullaniciId);

            return sorgu.Count();
        }


        public int ToplamFirsatSayisi(int? kullaniciId = null)
        {
            var sorgu = _context.Firsatlars.Where(f => f.SilindiMi == false || f.SilindiMi == null);
            if (kullaniciId.HasValue)
                sorgu = sorgu.Where(f => f.Musteri != null && f.Musteri.KullaniciId == kullaniciId);

            return sorgu.Count();
        }


        public decimal? ToplamTahminiGelir(int? kullaniciId = null)
        {
            var sorgu = _context.Firsatlars.Where(f => f.SilindiMi == false || f.SilindiMi == null);
            if (kullaniciId.HasValue)
                sorgu = sorgu.Where(f => f.Musteri != null && f.Musteri.KullaniciId == kullaniciId);

            return sorgu.Sum(f => f.TahminiGelir) ?? 0;
        }

        public Dictionary<string, int> GorevDurumlari(int? kullaniciId = null)
        {
            var sorgu = _context.Gorevlers.Where(g => g.SilindiMi == false || g.SilindiMi == null);

            if (kullaniciId.HasValue)
                sorgu = sorgu.Where(g => g.SorumluKullaniciId == kullaniciId);

            return sorgu
                .GroupBy(g => g.Durum ?? "Bilinmiyor")
                .ToDictionary(g => g.Key, g => g.Count());
        }


    }
}
