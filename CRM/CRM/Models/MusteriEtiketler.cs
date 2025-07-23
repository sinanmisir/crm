using System;
using System.Collections.Generic;

namespace CRM.Models;

public partial class MusteriEtiketler
{
    public int Id { get; set; }

    public int MusteriId { get; set; }

    public int EtiketId { get; set; }

    public virtual Etiketler Etiket { get; set; } = null!;

    public virtual Musteriler Musteri { get; set; } = null!;
}
