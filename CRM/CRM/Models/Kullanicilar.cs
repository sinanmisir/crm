using System;
using System.Collections.Generic;

namespace CRM.Models;

public partial class Kullanicilar
{
    public int Id { get; set; }

    public string? AdSoyad { get; set; }

    public string? KullaniciAdi { get; set; }

    public string? Eposta { get; set; }

    public string? SifreHash { get; set; }

    public string? Rol { get; set; }

    public virtual ICollection<Gorevler> Gorevlers { get; set; } = new List<Gorevler>();
}
