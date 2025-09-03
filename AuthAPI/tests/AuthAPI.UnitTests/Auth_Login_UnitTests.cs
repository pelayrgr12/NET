using Xunit;                       
using Microsoft.AspNetCore.Mvc;     
using Microsoft.AspNetCore.Http;    
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; 
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using AuthAPI.Controllers;          
using AuthAPI.DTO;                  
using AuthAPI.Utils;                
using AuthAPI.Models;               
using Isopoh.Cryptography.Argon2;   

namespace AuthAPI.UnitTests;

public class Auth_Login_UnitTests
{
  
    private static IConfiguration BuildJwtConfigSafe()
    {
        // Diccionario con claves típicas de JWT necesarias para AuthController
        var dict = new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=", // Clave secreta en Base64
            ["Jwt:Issuer"] = "AuthAPI",              // Quién emite el token
            ["Jwt:Audience"] = "AuthAPI.Clients",    // Quién lo consume
            ["Jwt:DurationInMinutes"] = "60"         // Duración del token (para que no falle el int.Parse)
        };

        // Por si el controlador intenta leer desde variables de entorno
        Environment.SetEnvironmentVariable("Jwt__ExpireMinutes", "60");
        Environment.SetEnvironmentVariable("Jwt__ExpirationMinutes", "60");

        // Construimos un IConfiguration en memoria con esas claves
        return new ConfigurationBuilder()
            .AddInMemoryCollection(dict)
            .AddEnvironmentVariables() 
            .Build();
    }

    private static AuthController BuildController(AppDbContext db, IConfiguration cfg)
    {
        var controller = new AuthController(db, cfg);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()     //el controlador escribe cookies en Response
        };
        return controller;
    }

    private static async Task SeedUserAsync(AppDbContext db, string correo, string contrasenPlana)
    {
        var hash = Argon2.Hash(contrasenPlana); 
        db.Usuarios.Add(new Usuario
        {
            Nombre = "Test",
            Correo = correo,
            contrasena = hash,
            Edad = 25,
            Rol = new TipoRol { Id = 1, NombreRol = "Usuario" }
        });
        await db.SaveChangesAsync();
    }

    // TEST 1: login correcto debe devolver 200 OK
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOk()
    {
        // Creamos un DbContext en memoria (una "base de datos fake")
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"unit-db-login-ok-{Guid.NewGuid()}") // nombre único
            .Options;

        using var db = new AppDbContext(options);
        var cfg = BuildJwtConfigSafe();

        const string correo = "correo@pruebatest.com";
        const string password    = "Password123";
        await SeedUserAsync(db, correo, password); // guardamos el usuario

        var controller = BuildController(db, cfg);

        var dto = new LoginDTO { Correo = correo, Contrasena = password };

        // Act: ejecutamos el login
        var result = await controller.Login(dto);

        // Assert: comprobamos que devuelve OkObjectResult
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value); // El contenido (ej: token) no debe ser null
    }

    // TEST 2: login con contraseña incorrecta debe devolver 401 Unauthorized
    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"unit-db-login-bad-{Guid.NewGuid()}")
            .Options;

        using var db = new AppDbContext(options);
        var cfg = BuildJwtConfigSafe();

        const string correo = "correo@pruebatest.com";
    
        await SeedUserAsync(db, correo, "Password123");

        var controller = BuildController(db, cfg);

        // Intentamos login con contraseña incorrecta
        var dto = new LoginDTO { Correo = correo, Contrasena = "mal" };

        var result = await controller.Login(dto);

        // Assert: debe devolver UnauthorizedObjectResult
        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}
