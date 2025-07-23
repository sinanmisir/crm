using AutoMapper;
using CRM.DTOs;
using CRM.Models;

namespace CRM.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Gorevler, GorevDTO>()
                .ForMember(dest => dest.MusteriAd, opt => opt.MapFrom(src => src.Musteri != null ? src.Musteri.AdSoyad : "Yok"))
                .ForMember(dest => dest.KisiAd, opt => opt.MapFrom(src => src.Kisi != null ? src.Kisi.AdSoyad : "Yok"))
                .ForMember(dest => dest.SorumluKullaniciAd, opt => opt.MapFrom(src => src.SorumluKullanici != null ? src.SorumluKullanici.AdSoyad : "Yok"))
                .ForMember(dest => dest.SonTarih, opt => opt.MapFrom(src => src.SonTarih.HasValue ? src.SonTarih.Value.ToDateTime(new TimeOnly(0, 0)) : (DateTime?)null));

            CreateMap<Notlar, NotDTO>()
                .ForMember(dest => dest.MusteriAd, opt => opt.MapFrom(src => src.Musteri != null ? src.Musteri.AdSoyad : "Yok"))
                .ForMember(dest => dest.KisiAd, opt => opt.MapFrom(src => src.Kisi != null ? src.Kisi.AdSoyad : "Yok"));

            CreateMap<Kisiler, KisiDTO>();

            CreateMap<Musteriler, MusteriDTO>();

            CreateMap<Firsatlar, FirsatDTO>()
                .ForMember(dest => dest.MusteriAd, opt => opt.MapFrom(src => src.Musteri != null ? src.Musteri.AdSoyad : "Yok"))
                .ForMember(dest => dest.Tarih, opt => opt.MapFrom(src => src.Tarih.HasValue ? src.Tarih.Value.ToDateTime(new TimeOnly(0, 0)) : (DateTime?)null))
                .ForMember(dest => dest.Asama, opt => opt.MapFrom(src => src.Asama.HasValue ? (FirsatAsama)src.Asama.Value : (FirsatAsama?)null));
            CreateMap<Etiketler, EtiketDTO>();
            CreateMap<IslemLog, IslemLogDTO>();
        }
    }
}
