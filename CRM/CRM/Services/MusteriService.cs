using AutoMapper;
using CRM.DTOs;
using CRM.Models;
using Microsoft.EntityFrameworkCore;

namespace CRM.Services
{
    public class MusteriService
    {
        private readonly CrmDbContext _context;
        private readonly IMapper _mapper;

        public MusteriService(CrmDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<MusteriDTO> GetAllMusteriler(int? kullaniciId = null, string? filtre = null, int? etiketId = null)
        {
            var sorgu = _context.Musterilers
                .Include(m => m.MusteriEtiketlers)
                    .ThenInclude(me => me.Etiket)
                .Where(m => m.SilindiMi == false || m.SilindiMi == null);

            if (kullaniciId.HasValue)
                sorgu = sorgu.Where(m => m.KullaniciId == kullaniciId);

            if (!string.IsNullOrWhiteSpace(filtre))
                sorgu = sorgu.Where(m =>
                    m.AdSoyad!.Contains(filtre) ||
                    m.Eposta!.Contains(filtre) ||
                    m.Telefon!.Contains(filtre));

            if (etiketId.HasValue)
                sorgu = sorgu.Where(m => m.MusteriEtiketlers.Any(me => me.EtiketId == etiketId.Value));

            return sorgu
                .AsEnumerable()
                .Select(m =>
                {
                    var dto = _mapper.Map<MusteriDTO>(m);
                    dto.Etiketler = m.MusteriEtiketlers.Select(e => e.Etiket.Ad).ToList();
                    return dto;
                })
                .ToList();
        }

        public MusteriDTO? GetMusteriById(int id)
        {
            var musteri = _context.Musterilers
                .Include(m => m.Gorevlers)
                    .ThenInclude(g => g.Kisi)
                .Include(m => m.Gorevlers)
                    .ThenInclude(g => g.SorumluKullanici)
                .Include(m => m.Notlars)
                .Include(m => m.Firsatlars)
                .Include(m => m.Kisilers)
                .Include(m => m.MusteriEtiketlers)
                    .ThenInclude(me => me.Etiket)
                .Include(m => m.IslemLogs) // ✅ Loglar dahil edildi
                .FirstOrDefault(m => m.Id == id && (m.SilindiMi == false || m.SilindiMi == null));

            if (musteri == null) return null;

            var dto = _mapper.Map<MusteriDTO>(musteri);

            dto.Gorevler = musteri.Gorevlers
                .Where(g => g.SilindiMi == false || g.SilindiMi == null)
                .Select(g => _mapper.Map<GorevDTO>(g)).ToList();

            dto.Notlar = musteri.Notlars
                .Where(n => n.SilindiMi == false || n.SilindiMi == null)
                .Select(n => _mapper.Map<NotDTO>(n)).ToList();

            dto.Firsatlar = musteri.Firsatlars
                .Where(f => f.SilindiMi == false || f.SilindiMi == null)
                .Select(f => _mapper.Map<FirsatDTO>(f)).ToList();

            dto.Kisiler = musteri.Kisilers
                .Where(k => k.SilindiMi == false || k.SilindiMi == null)
                .Select(k => _mapper.Map<KisiDTO>(k)).ToList();

            dto.Etiketler = musteri.MusteriEtiketlers
                .Select(e => e.Etiket.Ad)
                .ToList();

            dto.IslemLoglari = musteri.IslemLogs
                .OrderByDescending(l => l.Tarih)
                .Select(l => _mapper.Map<IslemLogDTO>(l))
                .ToList();

            return dto;
        }
    }
}
