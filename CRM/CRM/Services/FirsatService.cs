using CRM.Models;
using CRM.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CRM.Services
{
    public class FirsatService
    {
        private readonly CrmDbContext _context;

        public FirsatService(CrmDbContext context)
        {
            _context = context;
        }

        public List<FirsatDTO> GetAllFirsatlar(int? kullaniciId = null)
        {
            var sorgu = _context.Firsatlars
                .Where(f => f.SilindiMi == false || f.SilindiMi == null);

            if (kullaniciId.HasValue)
            {
                sorgu = sorgu.Where(f => f.Musteri != null && f.Musteri.KullaniciId == kullaniciId);
            }

            return sorgu
                .Select(f => new FirsatDTO
                {
                    Id = f.Id,
                    MusteriAd = f.Musteri != null ? f.Musteri.AdSoyad : "Yok",
                    Baslik = f.Baslik,
                    // Enum'dan string'e dönüştürülüyor (ToString kullanıyoruz)
                    Asama = f.Asama.HasValue ? (FirsatAsama?)f.Asama.Value : null,

                    TahminiGelir = f.TahminiGelir,
                    Tarih = f.Tarih.HasValue ? f.Tarih.Value.ToDateTime(new TimeOnly()) : null
                })
                .ToList();
        }

        public void FirsatEkle(FirsatEkleDTO dto)
        {
            var firsat = new Firsatlar
            {
                MusteriId = dto.MusteriId,
                Baslik = dto.Baslik,
                // Enum'u int? olarak kaydediyoruz
                Asama = dto.Asama.HasValue ? (int?)dto.Asama.Value : null,
                TahminiGelir = dto.TahminiGelir,
                Tarih = dto.Tarih.HasValue ? DateOnly.FromDateTime(dto.Tarih.Value) : null
            };

            _context.Firsatlars.Add(firsat);
            _context.SaveChanges();
        }

        public List<SelectListItem> GetMusteriSelectList()
        {
            return _context.Musterilers
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.AdSoyad
                }).ToList();
        }

        public FirsatGuncelleDTO? GetFirsatById(int id)
        {
            return _context.Firsatlars
                .Where(f => f.Id == id)
                .Select(f => new FirsatGuncelleDTO
                {
                    Id = f.Id,
                    MusteriId = f.MusteriId,
                    Baslik = f.Baslik,
                    // int? → Enum? dönüşümü (Cast)
                    Asama = f.Asama.HasValue ? (FirsatAsama?)f.Asama.Value : null,
                    TahminiGelir = f.TahminiGelir,
                    Tarih = f.Tarih.HasValue ? f.Tarih.Value.ToDateTime(new TimeOnly()) : null
                })
                .FirstOrDefault();
        }

        public void GuncelleFirsat(FirsatGuncelleDTO dto)
        {
            var firsat = _context.Firsatlars.Find(dto.Id);
            if (firsat == null) return;

            firsat.MusteriId = dto.MusteriId;
            firsat.Baslik = dto.Baslik;
            // Enum'u int olarak kaydediyoruz
            firsat.Asama = dto.Asama.HasValue ? (int?)dto.Asama.Value : null;
            firsat.TahminiGelir = dto.TahminiGelir;
            firsat.Tarih = dto.Tarih.HasValue ? DateOnly.FromDateTime(dto.Tarih.Value) : null;

            _context.SaveChanges();
        }

        public void FirsatSil(int id)
        {
            var firsat = _context.Firsatlars.Find(id);
            if (firsat != null)
            {
                firsat.SilindiMi = true;
                _context.SaveChanges();
            }
        }
    }
}
