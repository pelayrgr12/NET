using System;
using System.Collections.Generic;

namespace AuthAPI.src.Models;

public partial class Marca
{
    public ulong id_marca { get; set; }

    public string nombre { get; set; } = null!;

    public string? pais { get; set; }

    public virtual ICollection<Modelo> modelos { get; set; } = new List<Modelo>();
}
