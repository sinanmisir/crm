using System;
using System.Collections.Generic;

namespace CRM.Models;

public partial class Gorevler
{
    public int Id { get; set; }

    public int? MusteriId { get; set; }

    public int? KisiId { get; set; }

    public int? SorumluKullaniciId { get; set; }

    public string? Aciklama { get; set; }

    public DateOnly? SonTarih { get; set; }

    public string? Durum { get; set; }

    public bool? SilindiMi { get; set; }

    public virtual Kisiler? Kisi { get; set; }

    public virtual Musteriler? Musteri { get; set; }

    public virtual Kullanicilar? SorumluKullanici { get; set; }
}
