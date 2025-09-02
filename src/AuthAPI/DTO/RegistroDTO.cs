using AuthAPI.Models;

namespace AuthAPI.DTO
{
    public class RegistroDTO
    {
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public int Edad { get; set; }
        public int IdRol { get; set; }    
    }
}