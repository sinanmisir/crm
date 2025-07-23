using System;
using System.Collections.Generic;

namespace CRM.Models;

public partial class Etiketler
{
    public int Id { get; set; }

    public string Ad { get; set; } = null!;

    public virtual ICollection<MusteriEtiketler> MusteriEtiketlers { get; set; } = new List<MusteriEtiketler>();
}
