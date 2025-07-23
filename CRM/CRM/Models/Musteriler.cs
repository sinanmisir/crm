using System;
using System.Collections.Generic;

namespace CRM.Models;

public partial class Musteriler
{
    public int Id { get; set; }

    public string? AdSoyad { get; set; }

    public string? Eposta { get; set; }

    public string? Telefon { get; set; }

    public string? Adres { get; set; }

    public DateTime? KayitTarihi { get; set; }

    public bool? SilindiMi { get; set; }

    public int? KullaniciId { get; set; }

    public virtual ICollection<Firsatlar> Firsatlars { get; set; } = new List<Firsatlar>();

    public virtual ICollection<Gorevler> Gorevlers { get; set; } = new List<Gorevler>();

    public virtual ICollection<IslemLog> IslemLogs { get; set; } = new List<IslemLog>();

    public virtual ICollection<Kisiler> Kisilers { get; set; } = new List<Kisiler>();

    public virtual ICollection<MusteriEtiketler> MusteriEtiketlers { get; set; } = new List<MusteriEtiketler>();

    public virtual ICollection<Notlar> Notlars { get; set; } = new List<Notlar>();
}
