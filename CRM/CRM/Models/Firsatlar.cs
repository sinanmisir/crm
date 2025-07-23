using System;
using System.Collections.Generic;

namespace CRM.Models;

public partial class Firsatlar
{
    public int Id { get; set; }

    public int? MusteriId { get; set; }

    public string? Baslik { get; set; }

    public decimal? TahminiGelir { get; set; }

    public DateOnly? Tarih { get; set; }

    public bool? SilindiMi { get; set; }

    public int? Asama { get; set; }

    public virtual Musteriler? Musteri { get; set; }
}
