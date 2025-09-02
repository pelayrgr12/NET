using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------- EF Core (MySQL) ----------
var cs = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AuthAPI.AppDbContext>(opts =>
{
    var serverVersion = ServerVersion.AutoDetect(cs);
    opts.UseMySql(cs, serverVersion);
});

// ---------- JWT ----------
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["Key"]!)
            )
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                if (ctx.Request.Cookies.TryGetValue("jwt", out var token))
                    ctx.Token = token;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// ---------- CORS ----------
builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowAll", p =>
        p.SetIsOriginAllowed(_ => true)
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials());
});

// ---------- Swagger  ----------
builder.Services.AddEndpointsApiExplorer(); // explorador de endpoints
builder.Services.AddSwaggerGen();

// ---------- Controllers ----------
builder.Services.AddControllers();

var app = builder.Build();

// ---------- Swagger ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthAPI v1");
        c.RoutePrefix = "swagger"; // UI en /swagger
    });
}

// Para poder ver el swagger http://localhost:5210/swagger

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
