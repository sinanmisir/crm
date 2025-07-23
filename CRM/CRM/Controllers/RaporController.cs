using CRM.Models;
using CRM.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.Controllers
{
    public class RaporController : Controller
    {
        private readonly RaporService _service;
        private readonly CrmDbContext _context;
        public RaporController(CrmDbContext context)
        {
            _context = context;
            _service = new RaporService(context);
        }

        public IActionResult Index()
        {
            int? kullaniciId = null;

            if (!User.IsInRole("Admin"))
            {
                var kullaniciAdi = User.Identity.Name;
                var user = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == kullaniciAdi);
                kullaniciId = user?.Id;
            }

            var model = new
            {
                MusteriSayisi = _service.ToplamMusteriSayisi(kullaniciId),
                GorevSayisi = _service.ToplamGorevSayisi(kullaniciId),
                TamamlananGorevSayisi = _service.TamamlananGorevSayisi(kullaniciId),
                FirsatSayisi = _service.ToplamFirsatSayisi(kullaniciId),
                ToplamGelir = _service.ToplamTahminiGelir(kullaniciId),
                GorevDurumlari = _service.GorevDurumlari(kullaniciId)
            };

            return View(model);
        }


    }
}
