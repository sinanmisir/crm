using System;
using System.Collections.Generic;

namespace CRM.Models;

public partial class Notlar
{
    public int Id { get; set; }

    public int? MusteriId { get; set; }

    public int? KisiId { get; set; }

    public string? Baslik { get; set; }

    public string? Icerik { get; set; }

    public DateTime? Tarih { get; set; }

    public bool? SilindiMi { get; set; }

    public virtual Kisiler? Kisi { get; set; }

    public virtual Musteriler? Musteri { get; set; }
}
