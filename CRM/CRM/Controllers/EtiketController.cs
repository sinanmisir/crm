using AutoMapper;
using CRM.Services;
using CRM.DTOs;
using CRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EtiketController : Controller
    {
        private readonly EtiketService _service;
        private readonly CrmDbContext _context;
        public EtiketController(CrmDbContext context, IMapper mapper)
        {
            _context = context;
            _service = new EtiketService(context, mapper);
        }

        public IActionResult Index()
        {
            var etiketler = _service.GetAll();
            return View(etiketler);
        }

        [HttpPost]
        public IActionResult Ekle(string ad)
        {
            if (!string.IsNullOrWhiteSpace(ad))
                _service.Ekle(ad);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Guncelle(int id, string ad)
        {
            _service.Guncelle(id, ad);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Sil(int id)
        {
            var etiket = _context.Etiketlers.Find(id);
            if (etiket == null)
                return NotFound();

            var kullanilan = _context.MusteriEtiketlers.Any(me => me.EtiketId == id);
            if (kullanilan)
            {
                TempData["SilUyari"] = "Bu etiket bazı müşterilerde kullanılıyor. Silmek tüm ilişkileri de kaldırır.";
                TempData["SilEtiketId"] = id;
                TempData["SilEtiketAd"] = etiket.Ad;
                return RedirectToAction("Index");
            }

            _context.Etiketlers.Remove(etiket);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SilOnayla(int id)
        {
            var etiket = _context.Etiketlers.Find(id);
            if (etiket == null) return NotFound();

            var baglantilar = _context.MusteriEtiketlers.Where(me => me.EtiketId == id);
            _context.MusteriEtiketlers.RemoveRange(baglantilar); // önce bağlantıları sil

            _context.Etiketlers.Remove(etiket); // sonra etiketi sil
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


    }
}
