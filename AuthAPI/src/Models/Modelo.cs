using System;
using System.Collections.Generic;

namespace AuthAPI.src.Models;

public partial class Modelo
{
    public ulong id_modelo { get; set; }

    public ulong id_marca { get; set; }

    public string nombre { get; set; } = null!;

    public short? anio_inicio { get; set; }

    public short? anio_fin { get; set; }

    public virtual ICollection<Coche> Coches { get; set; } = new List<Coche>();

    public virtual Marca Id_marcaNavigation { get; set; } = null!;
}
