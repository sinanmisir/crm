using CRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.Controllers
{
    [Authorize]
    public class KisiController : Controller
    {
        private readonly CrmDbContext _context;

        public KisiController(CrmDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var kisiler = _context.Kisilers
                .Include(k => k.Musteri)
                .ToList();

            return View(kisiler);
        }
        [HttpGet]
        public IActionResult Ekle()
        {
            ViewBag.Musteriler = _context.Musterilers
                .Select(m => new { m.Id, m.AdSoyad })
                .ToList();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Ekle(Kisiler model)
        {
            if (ModelState.IsValid)
            {
                _context.Kisilers.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Musteriler = _context.Musterilers
                .Select(m => new { m.Id, m.AdSoyad })
                .ToList();

            return View(model);
        }
        [HttpGet]
        public IActionResult Guncelle(int id)
        {
            var kisi = _context.Kisilers.Find(id);
            if (kisi == null)
                return NotFound();

            ViewBag.Musteriler = _context.Musterilers
                .Select(m => new { m.Id, m.AdSoyad })
                .ToList();

            return View(kisi);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Guncelle(Kisiler model)
        {
            if (ModelState.IsValid)
            {
                _context.Kisilers.Update(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Musteriler = _context.Musterilers
                .Select(m => new { m.Id, m.AdSoyad })
                .ToList();

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Sil(int id)
        {
            var kisi = _context.Kisilers
                .Include(k => k.Gorevlers)
                .Include(k => k.Notlars)
                .FirstOrDefault(k => k.Id == id);

            if (kisi == null)
                return NotFound();

            if (kisi.Gorevlers.Any() || kisi.Notlars.Any())
            {
                TempData["Hata"] = "Bu kişiye bağlı görev veya not olduğu için silinemez.";
                return RedirectToAction("Index");
            }

            kisi.SilindiMi = true;  // Soft Delete ✔
            _context.SaveChanges();

            return RedirectToAction("Index");
        }



    }
}
