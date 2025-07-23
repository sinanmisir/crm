using AutoMapper;
using CRM.DTOs;
using CRM.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CRM.Services
{
    public class NotService
    {
        private readonly CrmDbContext _context;
        private readonly IMapper _mapper;

        public NotService(CrmDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public void NotEkle(NotEkleDTO dto)
        {
            if (dto.MusteriId == null && dto.KisiId == null)
                throw new InvalidOperationException("Not eklemek için en az bir Müşteri veya Kişi seçmelisiniz.");

            var not = new Notlar
            {
                MusteriId = dto.MusteriId,
                KisiId = dto.KisiId,
                Baslik = dto.Baslik,
                Icerik = dto.Icerik,
                Tarih = dto.Tarih ?? DateTime.Now
            };

            _context.Notlars.Add(not);
            _context.SaveChanges();
        }

        public List<SelectListItem> GetMusteriSelectList()
        {
            return _context.Musterilers
                .Where(m => m.SilindiMi == false || m.SilindiMi == null)
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.AdSoyad
                }).ToList();
        }

        public List<SelectListItem> GetKisiSelectList()
        {
            return _context.Kisilers
                .Where(k => k.SilindiMi == false || k.SilindiMi == null)
                .Select(k => new SelectListItem
                {
                    Value = k.Id.ToString(),
                    Text = k.AdSoyad
                }).ToList();
        }

        public List<NotDTO> GetAllNotlar(int? kullaniciId = null)
        {
            var sorgu = _context.Notlars
                .Include(n => n.Musteri)
                .Include(n => n.Kisi)
                .Where(n => n.SilindiMi == false || n.SilindiMi == null);

            if (kullaniciId.HasValue)
                sorgu = sorgu.Where(n => n.Musteri != null && n.Musteri.KullaniciId == kullaniciId);

            var liste = sorgu.ToList();

            return liste.Select(n => new NotDTO
            {
                Id = n.Id,
                MusteriAd = n.Musteri != null ? n.Musteri.AdSoyad : "Yok",
                KisiAd = n.Kisi != null ? n.Kisi.AdSoyad : "Yok",
                Baslik = n.Baslik,
                Icerik = n.Icerik,
                Tarih = n.Tarih
            })
            .OrderByDescending(n => n.Tarih)
            .ToList();
        }


        public List<NotDTO> GetKisiNotlari(int kisiId)  // ✅ Yeni eklendi
        {
            return _context.Notlars
                .Where(n => n.KisiId == kisiId && (n.SilindiMi == false || n.SilindiMi == null))
                .OrderByDescending(n => n.Tarih)
                .Select(n => _mapper.Map<NotDTO>(n))
                .ToList();
        }

        public NotGuncelleDTO? GetNotById(int id)
        {
            return _context.Notlars
                .Where(n => n.Id == id)
                .Select(n => new NotGuncelleDTO
                {
                    Id = n.Id,
                    MusteriId = n.MusteriId,
                    KisiId = n.KisiId,
                    Baslik = n.Baslik,
                    Icerik = n.Icerik,
                    Tarih = n.Tarih
                })
                .FirstOrDefault();
        }

        public void GuncelleNot(NotGuncelleDTO dto)
        {
            var not = _context.Notlars.Find(dto.Id);
            if (not == null) return;

            not.MusteriId = dto.MusteriId;
            not.KisiId = dto.KisiId;
            not.Baslik = dto.Baslik;
            not.Icerik = dto.Icerik;
            not.Tarih = dto.Tarih;

            _context.SaveChanges();
        }

        public void NotSil(int id)
        {
            var not = _context.Notlars.Find(id);
            if (not != null)
            {
                not.SilindiMi = true;
                _context.SaveChanges();
            }
        }
    }
}
