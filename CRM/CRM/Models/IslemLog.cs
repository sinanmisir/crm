using System;
using System.Collections.Generic;

namespace CRM.Models;

public partial class IslemLog
{
    public int Id { get; set; }

    public int KullaniciId { get; set; }

    public string KullaniciAd { get; set; } = null!;

    public int? MusteriId { get; set; }

    public string? Islem { get; set; }

    public DateTime Tarih { get; set; }

    public virtual Musteriler? Musteri { get; set; }
}
