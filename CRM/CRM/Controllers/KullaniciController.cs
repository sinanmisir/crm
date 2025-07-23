using CRM.DTOs;
using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

public class KullaniciController : Controller
{
    private readonly CrmDbContext _context;

    public KullaniciController(CrmDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {
        var kullanicilar = _context.Kullanicilars.ToList();
        return View(kullanicilar);
    }
    [HttpGet]
    public IActionResult Ekle()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Ekle(KullaniciEkleDTO dto)
    {
        if (ModelState.IsValid)
        {
            var kullanici = new Kullanicilar
            {
                AdSoyad = dto.AdSoyad,
                Eposta = dto.Eposta,
                Rol = dto.Rol,
                SifreHash = ComputeSha256Hash(dto.Sifre!)  // ✔ Hashlenmiş şekilde kaydet
            };

            _context.Kullanicilars.Add(kullanici);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        return View(dto);
    }

    private string ComputeSha256Hash(string rawData)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
    }
}
