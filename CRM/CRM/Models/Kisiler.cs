using System;
using System.Collections.Generic;

namespace CRM.Models;

public partial class Kisiler
{
    public int Id { get; set; }

    public int? MusteriId { get; set; }

    public string? AdSoyad { get; set; }

    public string? Gorev { get; set; }

    public string? Telefon { get; set; }

    public string? Eposta { get; set; }

    public bool? SilindiMi { get; set; }

    public virtual ICollection<Gorevler> Gorevlers { get; set; } = new List<Gorevler>();

    public virtual Musteriler? Musteri { get; set; }

    public virtual ICollection<Notlar> Notlars { get; set; } = new List<Notlar>();
}
