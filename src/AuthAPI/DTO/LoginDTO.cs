using AuthAPI.Models;

namespace AuthAPI.DTO
{
    public class LoginDTO
    {
        public string Correo { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
    }
}