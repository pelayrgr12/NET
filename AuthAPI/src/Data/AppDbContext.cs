using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace AuthAPI.src.Models;

/*
Importante! Estos comandos son para regenerar el DbContext y las entidades a partir de la base de datos.
dotnet add package Pomelo.EntityFrameworkCore.MySql
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef

dotnet ef dbcontext scaffold "Name=DefaultConnection" Pomelo.EntityFrameworkCore.MySql \
  -o src/Models --context AppDbContext --use-database-names --force
*/

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Coche> Coches { get; set; }

    public virtual DbSet<Estados_coche> EstadosCoches { get; set; }

    public virtual DbSet<Marca> Marcas { get; set; }

    public virtual DbSet<Modelo> Modelos { get; set; }

    public virtual DbSet<TipoRol> TiposRoles { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql("name=DefaultConnection", Microsoft.EntityFrameworkCore.ServerVersion.Parse("10.4.28-mariadb"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Coche>(entity =>
        {
            entity.HasKey(e => e.id_coche).HasName("PRIMARY");

            entity.HasIndex(e => e.id_estado, "ix_coches_id_estado");

            entity.HasIndex(e => e.id_modelo, "ix_coches_id_modelo");

            entity.HasIndex(e => e.matricula, "uq_coches_matricula").IsUnique();

            entity.HasIndex(e => e.vin, "uq_coches_vin").IsUnique();

            entity.Property(e => e.id_coche).HasColumnType("bigint(20) unsigned");
            entity.Property(e => e.anio).HasColumnType("smallint(6)");
            entity.Property(e => e.color).HasMaxLength(40);
            entity.Property(e => e.combustible).HasMaxLength(20);
            entity.Property(e => e.id_estado).HasColumnType("bigint(20) unsigned");
            entity.Property(e => e.id_modelo).HasColumnType("bigint(20) unsigned");
            entity.Property(e => e.kilometros).HasColumnType("int(11)");
            entity.Property(e => e.matricula).HasMaxLength(15);
            entity.Property(e => e.observaciones).HasMaxLength(500);
            entity.Property(e => e.precio).HasPrecision(12, 2);
            entity.Property(e => e.transmision).HasMaxLength(20);
            entity.Property(e => e.vin).HasMaxLength(20);

            entity.HasOne(d => d.id_estadoNavigation).WithMany(p => p.Coches)
                .HasForeignKey(d => d.id_estado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_coches_estado");

            entity.HasOne(d => d.id_modeloNavigation).WithMany(p => p.Coches)
                .HasForeignKey(d => d.id_modelo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_coches_modelo");
        });

        modelBuilder.Entity<Estados_coche>(entity =>
        {
            entity.HasKey(e => e.id_estado).HasName("PRIMARY");

            entity.ToTable("estados_coche");

            entity.HasIndex(e => e.codigo, "uq_estados_codigo").IsUnique();

            entity.Property(e => e.id_estado).HasColumnType("bigint(20) unsigned");
            entity.Property(e => e.codigo).HasMaxLength(10);
            entity.Property(e => e.nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Marca>(entity =>
        {
            entity.HasKey(e => e.id_marca).HasName("PRIMARY");

            entity.HasIndex(e => e.nombre, "uq_marcas_nombre").IsUnique();

            entity.Property(e => e.id_marca).HasColumnType("bigint(20) unsigned");
            entity.Property(e => e.nombre).HasMaxLength(100);
            entity.Property(e => e.pais).HasMaxLength(100);
        });

        modelBuilder.Entity<Modelo>(entity =>
        {
            entity.HasKey(e => e.id_modelo).HasName("PRIMARY");

            entity.HasIndex(e => e.id_marca, "ix_modelos_id_marca");

            entity.HasIndex(e => new { e.id_marca, e.nombre }, "uq_modelos_marca_nombre").IsUnique();

            entity.Property(e => e.id_modelo).HasColumnType("bigint(20) unsigned");
            entity.Property(e => e.anio_fin).HasColumnType("smallint(6)");
            entity.Property(e => e.anio_inicio).HasColumnType("smallint(6)");
            entity.Property(e => e.id_marca).HasColumnType("bigint(20) unsigned");
            entity.Property(e => e.nombre).HasMaxLength(120);

            entity.HasOne(d => d.Id_marcaNavigation).WithMany(p => p.modelos)
                .HasForeignKey(d => d.id_marca)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_modelos_marca");
        });

        modelBuilder.Entity<TipoRol>(entity =>
        {
            entity.ToTable("tiposroles");
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.NombreRol).HasMaxLength(50);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            // ✅ índice sobre la FK, NO sobre la navegación
            entity.HasIndex(e => e.IdRol, "fk_usuarios_tiposroles");

            entity.HasIndex(e => e.Correo, "uq_usuarios_correo").IsUnique();

            entity.Property(e => e.Id).HasColumnType("int(11)");
            entity.Property(e => e.Nombre).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Correo).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Contrasena).HasMaxLength(255);
            entity.Property(e => e.Edad).HasColumnType("int(11)");
            entity.Property(e => e.IdRol).HasColumnType("int(11)");

            entity.HasOne(d => d.Rol)
                .WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_usuarios_tiposroles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
