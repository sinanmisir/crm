using CRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class IslemController : Controller
    {
        private readonly CrmDbContext _context;

        public IslemController(CrmDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var loglar = _context.IslemLogs
                .Include(l => l.Musteri)
                .OrderByDescending(l => l.Tarih)
                .Take(100)
                .ToList();

            return View(loglar);
        }
    }
}
