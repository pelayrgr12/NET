using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthAPI.DTO;
using AuthAPI.Models;
using AuthAPI.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;


namespace AuthAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _cfg;

    public AuthController(AppDbContext db, IConfiguration cfg)
    {
        _db = db;
        _cfg = cfg;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistroDTO dto)
    {
        var email = dto.Correo.Trim().ToLowerInvariant();

        if (await _db.Usuarios.AnyAsync(u => u.Correo == email))
            return BadRequest("El correo ya existe.");

        var user = new Usuario
        {
            Nombre = dto.Nombre,
            Correo = email,
            contrasena = PasswordHasher.Hash(dto.Contrasena),
            Edad = dto.Edad,
            IdRol = dto.IdRol     
        };

        _db.Usuarios.Add(user);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Usuario registrado" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Correo) || string.IsNullOrWhiteSpace(dto.Contrasena))
            return BadRequest(new { message = "Datos inválidos" });

        var email = dto.Correo.Trim().ToLowerInvariant();
        var usuario = await _db.Usuarios.Include(u => u.Rol)
                                        .FirstOrDefaultAsync(u => u.Correo == email);

        //Ese DUMMY está para igualar el tiempo de cálculo tanto si el usuario existe como si no,
        //  y así evitar que un atacante deduzca qué correos/usuarios son válidos.
        const string DUMMY = "$argon2id$v=19$m=65536,t=3,p=1$c29tZXNhbHQAAAAAAAAAAA$gX7sT4cU2pW8h1QfWq0n1w==";
        var hash = usuario?.contrasena ?? DUMMY;

        var ok = PasswordHasher.Verify(hash, dto.Contrasena);
        if (usuario == null || !ok)
            return Unauthorized(new { message = "Credenciales inválidas" });

        var token = CreateJwt(usuario);

        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddMinutes(int.Parse(_cfg["Jwt:DurationInMinutes"]!))
        });

            return Ok(new {
                message = "Login correcto",
                token = token   
            });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return Ok(new { message = "Logout" });
    }

   private string CreateJwt(Usuario user)
{
    // 1. Leer y convertir la clave secreta desde appsettings.json
    var keyBytes = Convert.FromBase64String(_cfg["Jwt:Key"]!);
    var securityKey = new SymmetricSecurityKey(keyBytes);

    // 2. Definir el algoritmo de firma (HS256 en este caso)
    var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    // 3. Definir los claims del usuario, los datos del usuario que irán en el token
    var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),   // id del usuario
        new Claim(JwtRegisteredClaimNames.Email, user.Correo),        // Email
        new Claim(ClaimTypes.Name, user.Nombre),                      // Nombre visible
        new Claim(ClaimTypes.Role, user.Rol?.NombreRol ?? "Usuario")  // Rol o "Usuario" por defecto
    };

    // 4. construimos el token
    var token = new JwtSecurityToken(
        issuer: _cfg["Jwt:Issuer"],        // quién emite el token
        audience: _cfg["Jwt:Audience"],    // quién puede usarlo
        claims: claims,
        notBefore: DateTime.UtcNow,        // el token no es válido antes de ahora
        expires: DateTime.UtcNow.AddMinutes(
            int.Parse(_cfg["Jwt:DurationInMinutes"] ?? "60")
        ),                                // duración configurable
        signingCredentials: creds
    );

    // 5. Serializar el token a string
    return new JwtSecurityTokenHandler().WriteToken(token);
}
}
