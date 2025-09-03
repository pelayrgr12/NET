using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AuthAPI.Controllers;
using AuthAPI.DTO;
using AuthAPI.Utils;

namespace AuthAPI.UnitTests;

public class Auth_Register_UnitTests
{
    [Fact]
    public async Task Register_ModelInvalido_Devuelve400()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("unit-db").Options;

        using var db = new AppDbContext(options);
        var cfg = new ConfigurationBuilder().Build();
        var controller = new AuthController(db, cfg);
        controller.ModelState.AddModelError("Correo", "Requerido");

        var dto = new RegistroDTO { Nombre = "", Correo = "", Contrasena = "", Edad = -1 };

        var result = await controller.Register(dto);

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
