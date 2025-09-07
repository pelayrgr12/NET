using System;
using System.Collections.Generic;

namespace AuthAPI.src.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string? Contrasena { get; set; }

    public int? Edad { get; set; }

    public int IdRol { get; set; }

    public virtual TipoRol Rol { get; set; } = null!;
}
    