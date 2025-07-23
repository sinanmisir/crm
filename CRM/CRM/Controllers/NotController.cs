using AutoMapper;
using CRM.DTOs;
using CRM.Models;
using CRM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
namespace CRM.Controllers
{
    [Authorize]
    public class NotController : Controller
    {
        private readonly NotService _service;
        private readonly CrmDbContext _context;
        private readonly IslemLogService _logService;

        public NotController(CrmDbContext context, IMapper mapper, IslemLogService logService)
        {
            _context = context;
            _service = new NotService(context, mapper);
            _logService = logService;
        }


        public IActionResult Index()
        {
            var kullaniciAdi = User.Identity?.Name;
            var user = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == kullaniciAdi);

            if (User.IsInRole("Admin"))
                return View(_service.GetAllNotlar());
            else
                return View(_service.GetAllNotlar(user?.Id));
        }

        public IActionResult KisiNotlari(int kisiId)
        {
            var notlar = _service.GetKisiNotlari(kisiId);
            return View(notlar);
        }



        // Formu göstermek için
        [HttpGet]
        public IActionResult Ekle(int? musteriId)
        {
            ViewBag.Musteriler = _service.GetMusteriSelectList();
            ViewBag.Kisiler = _service.GetKisiSelectList();

            var model = new NotEkleDTO
            {
                MusteriId = musteriId
            };

            return View(model);
        }



        // Formdan gelen veriyi kaydetmek için
        // POST: Not/Ekle
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Ekle([FromForm] NotEkleDTO dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _service.NotEkle(dto);

                    var kullaniciAdi = User.Identity?.Name;
                    var user = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == kullaniciAdi);
                    if (user != null && dto.MusteriId.HasValue)
                    {
                        _logService.Logla(user.Id, user.AdSoyad, $"📝 Not eklendi: {dto.Baslik}", dto.MusteriId);
                    }

                    return RedirectToAction("Index", "Musteri");
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            ViewBag.Musteriler = _service.GetMusteriSelectList();
            ViewBag.Kisiler = _service.GetKisiSelectList();
            return View(dto);
        }

        [HttpGet]
        public IActionResult Guncelle(int id)
        {
            var not = _service.GetNotById(id);
            if (not == null)
                return NotFound();

            ViewBag.Musteriler = _service.GetMusteriSelectList();
            ViewBag.Kisiler = _service.GetKisiSelectList();
            return View(not);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Guncelle([FromForm] NotGuncelleDTO dto)
        {
            if (ModelState.IsValid)
            {
                _service.GuncelleNot(dto);
                return RedirectToAction("Index", "Musteri");
            }

            ViewBag.Musteriler = _service.GetMusteriSelectList();
            ViewBag.Kisiler = _service.GetKisiSelectList();
            return View(dto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Sil(int id)
        {
            _service.NotSil(id);
            return RedirectToAction("Index", "Musteri");
        }

        
    }
}
