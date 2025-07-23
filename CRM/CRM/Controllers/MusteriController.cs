using AutoMapper;
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
    public class MusteriController : Controller
    {
        private readonly MusteriService _service;
        private readonly CrmDbContext _context;
        private readonly IslemLogService _logService;

        public MusteriController(CrmDbContext context, IMapper mapper, IslemLogService logService)
        {
            _context = context;
            _service = new MusteriService(context, mapper);
            _logService = logService;
        }

        public IActionResult Index(string? ara, int? etiketId)
        {
            ViewBag.Etiketler = _context.Etiketlers
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.Ad
                }).ToList();

            var kullaniciAdi = User.Identity?.Name;
            var user = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == kullaniciAdi);

            var musteriler = User.IsInRole("Admin")
                ? _service.GetAllMusteriler(null, ara, etiketId)
                : _service.GetAllMusteriler(user?.Id, ara, etiketId);

            ViewBag.Ara = ara;
            ViewBag.SeciliEtiketId = etiketId;

            return View(musteriler);
        }


        [HttpGet]
        public IActionResult Ekle()
        {
            ViewBag.Kullanicilar = _context.Kullanicilars
                .Select(k => new SelectListItem { Value = k.Id.ToString(), Text = k.AdSoyad }).ToList();

            ViewBag.Etiketler = new MultiSelectList(_context.Etiketlers, "Id", "Ad");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Ekle(MusteriEkleDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Kullanicilar = _context.Kullanicilars
                    .Select(k => new SelectListItem { Value = k.Id.ToString(), Text = k.AdSoyad }).ToList();
                ViewBag.Etiketler = new MultiSelectList(_context.Etiketlers, "Id", "Ad");
                return View(dto);
            }

            var musteri = new Musteriler
            {
                AdSoyad = dto.AdSoyad,
                Telefon = dto.Telefon,
                Eposta = dto.Eposta,
                Adres = dto.Adres,
                KullaniciId = dto.KullaniciId,
                KayitTarihi = DateTime.Now
            };

            _context.Musterilers.Add(musteri);
            _context.SaveChanges();

            var user = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == User.Identity.Name);
            if (user != null)
            {
                _logService.Logla(user.Id, user.AdSoyad, $"➕ Müşteri eklendi: {musteri.AdSoyad}", musteri.Id);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Guncelle(int id)
        {
            var musteri = _context.Musterilers
                .Include(m => m.MusteriEtiketlers)
                .FirstOrDefault(m => m.Id == id);

            if (musteri == null) return NotFound();

            var dto = new MusteriEkleDTO
            {
                AdSoyad = musteri.AdSoyad,
                Telefon = musteri.Telefon,
                Eposta = musteri.Eposta,
                Adres = musteri.Adres,
                KullaniciId = musteri.KullaniciId,
                SecilenEtiketler = musteri.MusteriEtiketlers.Select(me => me.EtiketId).ToList()
            };

            ViewBag.Kullanicilar = _context.Kullanicilars
                .Select(k => new SelectListItem { Value = k.Id.ToString(), Text = k.AdSoyad }).ToList();
            ViewBag.Etiketler = new MultiSelectList(_context.Etiketlers, "Id", "Ad", dto.SecilenEtiketler);
            ViewBag.MusteriId = id;

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Guncelle(int id, MusteriEkleDTO dto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Kullanicilar = _context.Kullanicilars
                    .Select(k => new SelectListItem { Value = k.Id.ToString(), Text = k.AdSoyad }).ToList();
                ViewBag.Etiketler = new MultiSelectList(_context.Etiketlers, "Id", "Ad", dto.SecilenEtiketler);
                ViewBag.MusteriId = id;
                return View(dto);
            }

            var musteri = _context.Musterilers.Find(id);
            if (musteri == null) return NotFound();

            musteri.AdSoyad = dto.AdSoyad;
            musteri.Telefon = dto.Telefon;
            musteri.Eposta = dto.Eposta;
            musteri.Adres = dto.Adres;
            musteri.KullaniciId = dto.KullaniciId;

            _context.Musterilers.Update(musteri);
            _context.SaveChanges();

            var eskiEtiketler = _context.MusteriEtiketlers.Where(me => me.MusteriId == id);
            _context.MusteriEtiketlers.RemoveRange(eskiEtiketler);
            _context.SaveChanges();

            foreach (var etiketId in dto.SecilenEtiketler)
            {
                _context.MusteriEtiketlers.Add(new MusteriEtiketler
                {
                    MusteriId = id,
                    EtiketId = etiketId
                });
            }

            _context.SaveChanges();

            var user = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == User.Identity.Name);
            if (user != null)
            {
                _logService.Logla(user.Id, user.AdSoyad, $"✏️ Müşteri güncellendi: {musteri.AdSoyad}", musteri.Id);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Sil(int id)
        {
            var musteri = _context.Musterilers.Find(id);
            if (musteri != null)
            {
                musteri.SilindiMi = true;
                _context.SaveChanges();

                var user = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == User.Identity.Name);
                if (user != null)
                {
                    _logService.Logla(user.Id, user.AdSoyad, $"🗑️ Müşteri silindi: {musteri.AdSoyad}", musteri.Id);
                }
            }

            return RedirectToAction("Index");
        }


        public IActionResult Detay(int id)
        {
            var musteri = _service.GetMusteriById(id);
            if (musteri == null) return NotFound();

            return View(musteri);
        }
    }
}
