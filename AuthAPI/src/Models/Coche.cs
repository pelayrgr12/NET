using System;
using System.Collections.Generic;

namespace AuthAPI.src.Models;

public partial class Coche
{
    public ulong id_coche { get; set; }

    public ulong id_modelo { get; set; }

    public ulong id_estado { get; set; }

    public string? vin { get; set; }

    public string? matricula { get; set; }

    public short? anio { get; set; }

    public string? color { get; set; }

    public string? combustible { get; set; }

    public string? transmision { get; set; }

    public int? kilometros { get; set; }

    public decimal? precio { get; set; }

    public string? observaciones { get; set; }

    public virtual Estados_coche id_estadoNavigation { get; set; } = null!;

    public virtual Modelo id_modeloNavigation { get; set; } = null!;
}
