using CRM.Models;

namespace CRM.Services
{
    public class IslemLogService
    {
        private readonly CrmDbContext _context;

        public IslemLogService(CrmDbContext context)
        {
            _context = context;
        }

        public void Logla(int kullaniciId, string kullaniciAd, string islem, int? musteriId = null)
        {
            var log = new IslemLog
            {
                KullaniciId = kullaniciId,
                KullaniciAd = kullaniciAd,
                Islem = islem,
                MusteriId = musteriId,
                Tarih = DateTime.Now
            };

            _context.IslemLogs.Add(log);
            _context.SaveChanges();
        }

        public List<IslemLog> GetLogs(int? musteriId = null)
        {
            var sorgu = _context.IslemLogs
                .Where(l => musteriId == null || l.MusteriId == musteriId)
                .OrderByDescending(l => l.Tarih);

            return sorgu.ToList();
        }
    }
}
