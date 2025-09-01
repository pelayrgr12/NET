using Microsoft.EntityFrameworkCore;
using AuthAPI.Models;

namespace AuthAPI;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<TipoRol> TiposRoles => Set<TipoRol>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TipoRol>(e =>
        {
            e.ToTable("TiposRoles");
            e.Property(p => p.NombreRol).HasMaxLength(50).IsRequired();
        });

      modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("Usuarios");
            e.HasIndex(u => u.Correo).IsUnique();
            e.Property(p => p.Nombre).HasMaxLength(100).IsRequired();
            e.Property(p => p.Correo).HasMaxLength(100).IsRequired();
            e.Property(p => p.contrasena).HasMaxLength(255).IsRequired();

            e.HasOne(u => u.Rol)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(u => u.IdRol)
            .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
