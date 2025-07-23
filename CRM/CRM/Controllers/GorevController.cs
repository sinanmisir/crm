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
    public class GorevController : Controller
    {
        private readonly GorevService _service;
        private readonly CrmDbContext _context;
        private readonly IslemLogService _logService;

        public GorevController(CrmDbContext context, IMapper mapper, IslemLogService logService)
        {
            _context = context;
            _service = new GorevService(context, mapper);
            _logService = logService;
        }

        public IActionResult Index()
        {
            var kullaniciAdi = User.Identity?.Name;
            var user = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == kullaniciAdi);
            var gorevler = User.IsInRole("Admin") ? _service.GetAllGorevler() : _service.GetAllGorevler(user?.Id);
            return View(gorevler);
        }

        public IActionResult AktifGorevler()
        {
            var kullaniciAdi = User.Identity?.Name;
            var user = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == kullaniciAdi);
            if (user == null) return RedirectToAction("Index");

            var aktifGorevler = _service.GetKullaniciAktifGorevler(user.Id);
            return View(aktifGorevler);
        }

        [HttpGet]
        public IActionResult Ekle()
        {
            var currentUserName = User.Identity?.Name;
            var currentUser = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == currentUserName);

            ViewBag.Kisiler = new SelectList(
                _context.Kisilers
                    .Where(k => k.SilindiMi == false || k.SilindiMi == null)
                    .ToList(),
                "Id", "AdSoyad"
            );

            ViewBag.Musteriler = new SelectList(
                _context.Musterilers
                    .Where(m => m.SilindiMi == false || m.SilindiMi == null)
                    .ToList(),
                "Id", "AdSoyad"
            );

            IEnumerable<Kullanicilar> atanabilirKullanicilar;
            if (currentUser != null && currentUser.Rol != "Admin")
            {
                atanabilirKullanicilar = _context.Kullanicilars
                    .Where(k => k.Rol != "Admin" && k.Id != currentUser.Id)
                    .ToList();
            }
            else
            {
                atanabilirKullanicilar = _context.Kullanicilars.ToList();
            }

            ViewBag.Kullanicilar = new SelectList(
                atanabilirKullanicilar,
                "Id", "AdSoyad"
            );

            ViewBag.Durumlar = new List<SelectListItem>
            {
                new SelectListItem { Text = "Bekliyor", Value = "Bekliyor" },
                new SelectListItem { Text = "Devam Ediyor", Value = "Devam Ediyor" },
                new SelectListItem { Text = "Tamamlandı", Value = "Tamamlandı" },
                new SelectListItem { Text = "İptal Edildi", Value = "İptal Edildi" }
            };

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Ekle(GorevEkleDTO dto)
        {
            if (dto.SonTarih.HasValue && dto.SonTarih.Value.Date < DateTime.Now.Date)
                ModelState.AddModelError("SonTarih", "Görev tarihi bugünden önce olamaz.");

            if (_service.GorevVarMi(dto.MusteriId, dto.KisiId, dto.SonTarih))
                ModelState.AddModelError("", "Bu kişiye bu tarihte zaten bir görev atanmış.");

            if (ModelState.IsValid)
            {
                _service.GorevEkle(dto);

                var user = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == User.Identity.Name);
                if (user != null)
                {
                    _logService.Logla(user.Id, user.AdSoyad, $"📌 Görev eklendi: {dto.Aciklama}", dto.MusteriId);
                }

                return RedirectToAction("Index", "Musteri");
            }


            var currentUserName = User.Identity?.Name;
            var currentUser = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == currentUserName);

            ViewBag.Musteriler = _context.Musterilers
                .Where(m => m.SilindiMi == false || m.SilindiMi == null)
                .Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.AdSoyad })
                .ToList();

            ViewBag.Kisiler = _context.Kisilers
                .Where(k => k.SilindiMi == false || k.SilindiMi == null)
                .Select(k => new SelectListItem { Value = k.Id.ToString(), Text = k.AdSoyad })
                .ToList();

            ViewBag.Kullanicilar = new SelectList(
                (currentUser != null && currentUser.Rol != "Admin")
                ? _context.Kullanicilars.Where(k => k.Rol != "Admin" && k.Id != currentUser.Id).ToList()
                : _context.Kullanicilars.ToList(),
                "Id", "AdSoyad"
            );

            return View(dto);
        }

        [HttpGet]
        public IActionResult Guncelle(int id)
        {
            var dto = _service.GetGorevById(id);
            if (dto == null) return NotFound();

            var currentUserName = User.Identity?.Name;
            var currentUser = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == currentUserName);

            ViewBag.Kisiler = new SelectList(
                _context.Kisilers
                    .Where(k => k.SilindiMi == false || k.SilindiMi == null)
                    .ToList(),
                "Id", "AdSoyad",
                dto.KisiId
            );

            ViewBag.Musteriler = new SelectList(
                _context.Musterilers
                    .Where(m => m.SilindiMi == false || m.SilindiMi == null)
                    .ToList(),
                "Id", "AdSoyad",
                dto.MusteriId
            );

            ViewBag.Kullanicilar = new SelectList(
                (currentUser != null && currentUser.Rol != "Admin")
                ? _context.Kullanicilars.Where(k => k.Rol != "Admin" && k.Id != currentUser.Id).ToList()
                : _context.Kullanicilars.ToList(),
                "Id", "AdSoyad",
                dto.SorumluKullaniciId
            );

            ViewBag.Durumlar = new List<SelectListItem>
            {
                new SelectListItem { Text = "Bekliyor", Value = "Bekliyor" },
                new SelectListItem { Text = "Devam Ediyor", Value = "Devam Ediyor" },
                new SelectListItem { Text = "Tamamlandı", Value = "Tamamlandı" },
                new SelectListItem { Text = "İptal Edildi", Value = "İptal Edildi" }
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Guncelle(GorevGuncelleDTO dto)
        {
            if (ModelState.IsValid)
            {
                var eski = _context.Gorevlers.AsNoTracking().FirstOrDefault(g => g.Id == dto.Id);
                var success = _service.GuncelleGorev(dto);
                if (success)
                {
                    var user = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == User.Identity.Name);
                    if (user != null)
                    {
                        var mesaj = $"📌 Görev güncellendi: {dto.Aciklama}";
                        if (eski?.Durum != dto.Durum && dto.Durum == "Tamamlandı")
                            mesaj = $"✅ Görev tamamlandı: {dto.Aciklama}";

                        _logService.Logla(user.Id, user.AdSoyad, mesaj, dto.MusteriId);
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Kayıt bulunamadı.");
                }
            }

            var currentUserName = User.Identity?.Name;
            var currentUser = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == currentUserName);

            ViewBag.Musteriler = _context.Musterilers
                .Where(m => m.SilindiMi == false || m.SilindiMi == null)
                .Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.AdSoyad })
                .ToList();

            ViewBag.Kisiler = _context.Kisilers
                .Where(k => k.SilindiMi == false || k.SilindiMi == null)
                .Select(k => new SelectListItem { Value = k.Id.ToString(), Text = k.AdSoyad })
                .ToList();

            ViewBag.Kullanicilar = new SelectList(
                (currentUser != null && currentUser.Rol != "Admin")
                ? _context.Kullanicilars.Where(k => k.Rol != "Admin" && k.Id != currentUser.Id).ToList()
                : _context.Kullanicilars.ToList(),
                "Id", "AdSoyad",
                dto.SorumluKullaniciId
            );

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Sil(int id)
        {
            var gorev = _context.Gorevlers.FirstOrDefault(g => g.Id == id);
            if (gorev != null)
            {
                gorev.SilindiMi = true;
                _context.SaveChanges();

                var user = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == User.Identity.Name);
                if (user != null)
                {
                    _logService.Logla(user.Id, user.AdSoyad, $"🗑️ Görev silindi: {gorev.Aciklama}", gorev.MusteriId);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Takvim()
        {
            var kullaniciAdi = User.Identity?.Name;
            var user = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == kullaniciAdi);
            var gorevler = _context.Gorevlers
                .Where(g => (g.SilindiMi == false || g.SilindiMi == null) &&
                            g.SonTarih.HasValue &&
                            (User.IsInRole("Admin") || g.SorumluKullaniciId == user.Id))
                .Include(g => g.Musteri)
                .Include(g => g.Kisi)
                .Include(g => g.SorumluKullanici)
                .ToList();

            var model = gorevler
                .Where(g => g.SonTarih.HasValue)
                .GroupBy(g => g.SonTarih.Value.DayOfWeek) // Pazartesi → DayOfWeek.Monday
                .ToDictionary(g => g.Key, g => g.ToList());

            return View("Takvim", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GunGuncelle([FromBody] GunGuncelleDTO dto)
        {
            var gorev = _context.Gorevlers.FirstOrDefault(g => g.Id == dto.Id);
            if (gorev == null) return NotFound();

            gorev.SonTarih = DateOnly.Parse(dto.YeniTarih);
            _context.SaveChanges();

            return Ok();
        }
        public IActionResult Ozet()
        {
            var kullaniciAdi = User.Identity?.Name;
            var user = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == kullaniciAdi);
            if (user == null) return Unauthorized();

            var model = new GorevOzetViewModel
            {
                Bugun = _service.GetBugunkuGorevler(user.Id),
                Yaklasan = _service.GetYaklasanGorevler(user.Id),
                Geciken = _service.GetGecikenGorevler(user.Id)
            };

            return View("Ozet", model);
        }


    }
}
