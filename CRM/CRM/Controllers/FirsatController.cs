using CRM.DTOs;
using CRM.Models;
using CRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CRM.Controllers
{
    [Authorize]
    public class FirsatController : Controller
    {
        private readonly FirsatService _service;
        private readonly CrmDbContext _context;
        private readonly IslemLogService _logService;

        public FirsatController(CrmDbContext context, IslemLogService logService)
        {
            _context = context;
            _service = new FirsatService(context);
            _logService = logService;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                var firsatlar = _service.GetAllFirsatlar();
                var gruplu = firsatlar
                    .GroupBy(f => f.Asama?.ToString() ?? "Belirtilmemiş")
                    .ToDictionary(g => g.Key, g => g.ToList());

                return View(gruplu);
            }
            else
            {
                var kullaniciAdi = User.Identity?.Name;
                var user = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == kullaniciAdi);
                var firsatlar = _service.GetAllFirsatlar(user?.Id);
                var gruplu = firsatlar
                    .GroupBy(f => f.Asama?.ToString() ?? "Belirtilmemiş")
                    .ToDictionary(g => g.Key, g => g.ToList());

                return View(gruplu);
            }
        }

        [HttpGet]
        public IActionResult Ekle(int? musteriId)
        {
            ViewBag.Musteriler = _service.GetMusteriSelectList();

            var model = new FirsatEkleDTO
            {
                MusteriId = musteriId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Ekle([FromForm] FirsatEkleDTO dto)
        {
            if (ModelState.IsValid)
            {
                _service.FirsatEkle(dto);

                var user = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == User.Identity.Name);
                if (user != null)
                {
                    _logService.Logla(user.Id, user.AdSoyad, $"💼 Fırsat eklendi: {dto.Baslik}", dto.MusteriId);
                }

                return RedirectToAction("Index", "Musteri");
            }

            ViewBag.Musteriler = _service.GetMusteriSelectList();
            return View(dto);
        }

        [HttpGet]
        public IActionResult Guncelle(int id)
        {
            var firsat = _service.GetFirsatById(id);
            if (firsat == null)
                return NotFound();

            ViewBag.Musteriler = _service.GetMusteriSelectList();
            return View(firsat);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Guncelle(FirsatGuncelleDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Musteriler = _service.GetMusteriSelectList();
                return View(dto);
            }

            var onceki = _context.Firsatlars
                .AsNoTracking()
                .FirstOrDefault(f => f.Id == dto.Id);

            _service.GuncelleFirsat(dto);

            var user = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == User.Identity.Name);
            if (user != null)
            {
                _logService.Logla(user.Id, user.AdSoyad, $"✏️ Fırsat güncellendi: {dto.Baslik}", dto.MusteriId);

                if (onceki?.Asama != (int)FirsatAsama.Kazanildi && dto.Asama == FirsatAsama.Kazanildi)
                {
                    _logService.Logla(user.Id, user.AdSoyad, $"🎯 Fırsat kazanıldı: {dto.Baslik}", dto.MusteriId);
                }
            }

            // Otomatik görev atama
            if (onceki?.Asama != (int)FirsatAsama.Kazanildi && dto.Asama == FirsatAsama.Kazanildi)
            {
                var musteri = _context.Musterilers.FirstOrDefault(m => m.Id == dto.MusteriId);
                var destekKullanici = _context.Kullanicilars.FirstOrDefault(k => k.Rol == "Kullanici");

                if (musteri != null && destekKullanici != null)
                {
                    var gorev = new Gorevler
                    {
                        MusteriId = dto.MusteriId,
                        SorumluKullaniciId = destekKullanici.Id,
                        Aciklama = "🎯 Kazanılan fırsat sonrası otomatik destek görevi",
                        SonTarih = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                        Durum = "Bekliyor"
                    };

                    _context.Gorevlers.Add(gorev);
                    _context.SaveChanges();

                    // Otomatik görev logu
                    if (user != null)
                    {
                        _logService.Logla(user.Id, user.AdSoyad, $"🛠️ Otomatik görev atandı: {gorev.Aciklama}", dto.MusteriId);
                    }
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Sil(int id)
        {
            var firsat = _context.Firsatlars.FirstOrDefault(f => f.Id == id);
            if (firsat != null)
            {
                firsat.SilindiMi = true;
                _context.SaveChanges();

                var user = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == User.Identity.Name);
                if (user != null)
                {
                    _logService.Logla(user.Id, user.AdSoyad, $"🗑️ Fırsat silindi: {firsat.Baslik}", firsat.MusteriId);
                }
            }

            return RedirectToAction("Index", "Musteri");
        }
    }
}
