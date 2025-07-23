// GorevService.cs (Loglu ve Tam)
using AutoMapper;
using CRM.DTOs;
using CRM.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CRM.Services
{
    public class GorevService
    {
        private readonly CrmDbContext _context;
        private readonly IMapper _mapper;

        public GorevService(CrmDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<GorevDTO> GetAllGorevler(int? kullaniciId = null)
        {
            var liste = _context.Gorevlers
                .Include(g => g.Musteri)
                .Include(g => g.Kisi)
                .Include(g => g.SorumluKullanici)
                .Where(g => g.SilindiMi == false || g.SilindiMi == null);

            if (kullaniciId.HasValue)
                liste = liste.Where(g => g.SorumluKullaniciId == kullaniciId);

            var gorevListesi = liste.ToList();  // Veritabanından çekiliyor

            return _mapper.Map<List<GorevDTO>>(gorevListesi);  // DTO'ya çevriliyor
        }

        public List<GorevDTO> GetKullaniciAktifGorevler(int kullaniciId)
        {
            var liste = _context.Gorevlers
                .Where(g => g.SorumluKullaniciId == kullaniciId &&
                            (g.SilindiMi == false || g.SilindiMi == null) &&
                            g.Durum != "Tamamlandı")
                .ToList();

            return _mapper.Map<List<GorevDTO>>(liste);
        }
        public List<GorevDTO> GetBugunkuGorevler(int kullaniciId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var gorevler = _context.Gorevlers
                .Include(g => g.Musteri)
                .Include(g => g.Kisi)
                .Include(g => g.SorumluKullanici)
                .Where(g => g.SorumluKullaniciId == kullaniciId &&
                            g.SonTarih == today &&
                            (g.SilindiMi == false || g.SilindiMi == null))
                .ToList();

            return _mapper.Map<List<GorevDTO>>(gorevler);
        }

        public List<GorevDTO> GetYaklasanGorevler(int kullaniciId)
        {
            var now = DateTime.Now;
            var tomorrow = now.AddHours(24);

            var gorevler = _context.Gorevlers
                .Include(g => g.Musteri)
                .Include(g => g.Kisi)
                .Include(g => g.SorumluKullanici)
                .Where(g => g.SorumluKullaniciId == kullaniciId &&
                            g.SonTarih >= DateOnly.FromDateTime(now) &&
                            g.SonTarih <= DateOnly.FromDateTime(tomorrow) &&
                            (g.SilindiMi == false || g.SilindiMi == null))
                .ToList();

            return _mapper.Map<List<GorevDTO>>(gorevler);
        }

        public List<GorevDTO> GetGecikenGorevler(int kullaniciId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var gorevler = _context.Gorevlers
                .Include(g => g.Musteri)
                .Include(g => g.Kisi)
                .Include(g => g.SorumluKullanici)
                .Where(g => g.SorumluKullaniciId == kullaniciId &&
                            g.SonTarih < today &&
                            g.Durum != "Tamamlandı" &&
                            (g.SilindiMi == false || g.SilindiMi == null))
                .ToList();

            return _mapper.Map<List<GorevDTO>>(gorevler);
        }

        public GorevGuncelleDTO? GetGorevById(int id)
        {
            return _context.Gorevlers
                .Where(g => g.Id == id)
                .Select(g => new GorevGuncelleDTO
                {
                    Id = g.Id,
                    MusteriId = g.MusteriId,
                    MusteriAd = g.Musteri != null ? g.Musteri.AdSoyad : "Yok",
                    KisiId = g.KisiId,
                    KisiAd = g.Kisi != null ? g.Kisi.AdSoyad : "Yok",
                    SorumluKullaniciId = g.SorumluKullaniciId,
                    SorumluKullaniciAd = g.SorumluKullanici != null ? g.SorumluKullanici.AdSoyad : "Yok",
                    Aciklama = g.Aciklama,
                    SonTarih = g.SonTarih.HasValue ? g.SonTarih.Value.ToDateTime(new TimeOnly(0, 0)) : null,
                    Durum = g.Durum
                })
                .FirstOrDefault();
        }

        public bool GuncelleGorev(GorevGuncelleDTO dto)
        {
            var gorev = _context.Gorevlers.Find(dto.Id);
            if (gorev == null) return false;

            gorev.MusteriId = dto.MusteriId;
            gorev.KisiId = dto.KisiId;
            gorev.SorumluKullaniciId = dto.SorumluKullaniciId;
            gorev.Aciklama = dto.Aciklama;
            gorev.SonTarih = dto.SonTarih.HasValue ? DateOnly.FromDateTime(dto.SonTarih.Value) : null;
            gorev.Durum = dto.Durum;

            _context.SaveChanges();
            return true;
        }


        public void GorevSil(int id)
        {
            var gorev = _context.Gorevlers.Find(id);
            if (gorev != null)
            {
                gorev.SilindiMi = true;
                _context.SaveChanges();
            }
        }

        public void GorevEkle(GorevEkleDTO dto)
        {
            var gorev = new Gorevler
            {
                MusteriId = dto.MusteriId,
                KisiId = dto.KisiId,
                SorumluKullaniciId = dto.SorumluKullaniciId,
                Aciklama = dto.Aciklama,
                SonTarih = dto.SonTarih.HasValue ? DateOnly.FromDateTime(dto.SonTarih.Value) : null,
                Durum = dto.Durum
            };

            _context.Gorevlers.Add(gorev);
            _context.SaveChanges();
        }

        public bool GorevVarMi(int? musteriId, int? kisiId, DateTime? sonTarih)
        {
            if (!sonTarih.HasValue || !kisiId.HasValue)
                return false;

            var mevcutGorev = _context.Gorevlers
                .FirstOrDefault(g => g.MusteriId == musteriId &&
                                     g.KisiId == kisiId &&
                                     g.SonTarih.HasValue &&
                                     g.SonTarih.Value == DateOnly.FromDateTime(sonTarih.Value) &&
                                     (g.SilindiMi == false || g.SilindiMi == null));

            return mevcutGorev != null;
        }

        public List<SelectListItem> GetMusteriSelectList() =>
            _context.Musterilers
                .Where(m => m.SilindiMi == false || m.SilindiMi == null)
                .Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.AdSoyad })
                .ToList();

        public List<SelectListItem> GetKullaniciSelectList() =>
            _context.Kullanicilars
                .Select(k => new SelectListItem { Value = k.Id.ToString(), Text = k.AdSoyad })
                .ToList();

        public List<SelectListItem> GetKisiSelectList() =>
            _context.Kisilers
                .Where(k => k.SilindiMi == false || k.SilindiMi == null)
                .Select(k => new SelectListItem { Value = k.Id.ToString(), Text = k.AdSoyad + (k.Musteri != null ? $" ({k.Musteri.AdSoyad})" : "") })
                .ToList();
    }


}