using System;
using System.Collections.Generic;

namespace AuthAPI.src.Models;

public partial class Estados_coche
{
    public ulong id_estado { get; set; }

    public string codigo { get; set; } = null!;

    public string nombre { get; set; } = null!;

    public virtual ICollection<Coche> Coches { get; set; } = new List<Coche>();
}
