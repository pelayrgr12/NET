using System;
using System.Collections.Generic;

namespace AuthAPI.src.Models;

public partial class TipoRol
{
    public int Id { get; set; }

    public string NombreRol { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
