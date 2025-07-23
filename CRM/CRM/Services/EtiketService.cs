using AutoMapper;
using CRM.DTOs;
using CRM.Models;

namespace CRM.Services
{
    public class EtiketService
    {
        private readonly CrmDbContext _context;
        private readonly IMapper _mapper;

        public EtiketService(CrmDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<EtiketDTO> GetAll()
        {
            return _context.Etiketlers
                .Select(e => _mapper.Map<EtiketDTO>(e))
                .ToList();
        }

        public void Ekle(string ad)
        {
            _context.Etiketlers.Add(new Etiketler { Ad = ad });
            _context.SaveChanges();
        }

        public void Guncelle(int id, string ad)
        {
            var etiket = _context.Etiketlers.Find(id);
            if (etiket != null)
            {
                etiket.Ad = ad;
                _context.SaveChanges();
            }
        }

        public void Sil(int id)
        {
            var etiket = _context.Etiketlers.Find(id);
            if (etiket != null)
            {
                _context.Etiketlers.Remove(etiket);
                _context.SaveChanges();
            }
        }
    }
}
