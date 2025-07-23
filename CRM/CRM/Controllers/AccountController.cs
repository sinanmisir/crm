using CRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using BCrypt.Net;

namespace CRM.Controllers
{
    public class AccountController : Controller
    {
        private readonly CrmDbContext _context;

        public AccountController(CrmDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Kullanicilars
                .FirstOrDefault(x => x.KullaniciAdi == model.KullaniciAdi);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.Sifre, user.SifreHash))
            {
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı");
                return View(model);
            }


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.KullaniciAdi),
                new Claim(ClaimTypes.Role, user.Rol ?? "Kullanici"),
                new Claim("AdSoyad", user.AdSoyad ?? "")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(Kullanicilar model)
        {
            if (ModelState.IsValid)
            {
                // Kullanıcı adı zaten var mı kontrol et
                var existing = _context.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == model.KullaniciAdi);
                if (existing != null)
                {
                    ModelState.AddModelError("KullaniciAdi", "Bu kullanıcı adı zaten kullanılıyor.");
                    return View(model);
                }

                // Şifreyi hashleyerek kaydet
                model.SifreHash = BCrypt.Net.BCrypt.HashPassword(model.SifreHash);

                model.Rol ??= "Kullanici"; // Rol belirtilmemişse varsayılan olarak 'Kullanici' ata

                _context.Kullanicilars.Add(model);
                _context.SaveChanges();

                return RedirectToAction("Login");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
