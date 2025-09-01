namespace AuthAPI.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string contrasena { get; set; } = string.Empty;
        public int Edad { get; set; }
        public int IdRol { get; set; }
        public TipoRol Rol { get; set; } = default!;

    }
}