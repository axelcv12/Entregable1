using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Aplicacion_Banco.Models
{
    public partial class APLICACION_BANCOContext : DbContext
    {
        public APLICACION_BANCOContext()
        {
        }

        public APLICACION_BANCOContext(DbContextOptions<APLICACION_BANCOContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Duracion> Duracions { get; set; } = null!;
        public virtual DbSet<Metodopago> Metodopagos { get; set; } = null!;
        public virtual DbSet<Monto> Montos { get; set; } = null!;
        public virtual DbSet<Prestamo> Prestamos { get; set; } = null!;
        public virtual DbSet<Rol> Rols { get; set; } = null!;
        public virtual DbSet<Usuario> Usuarios { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=(local); DataBase=APLICACION_BANCO;Integrated Security=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Duracion>(entity =>
            {
                entity.ToTable("DURACION");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Duracion1).HasColumnName("duracion");

                entity.Property(e => e.Interes)
                    .HasColumnType("decimal(10, 3)")
                    .HasColumnName("interes");
            });

            modelBuilder.Entity<Metodopago>(entity =>
            {
                entity.ToTable("METODOPAGO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("nombre");
            });

            modelBuilder.Entity<Monto>(entity =>
            {
                entity.ToTable("MONTO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Valor)
                    .HasColumnType("decimal(18, 0)")
                    .HasColumnName("valor");
            });

            modelBuilder.Entity<Prestamo>(entity =>
            {
                entity.ToTable("PRESTAMO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Estado)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("estado");

                entity.Property(e => e.FechaFin)
                    .HasColumnType("datetime")
                    .HasColumnName("fechaFin");

                entity.Property(e => e.FechaInicio)
                    .HasColumnType("datetime")
                    .HasColumnName("fechaInicio");

                entity.Property(e => e.IdDuracion).HasColumnName("idDuracion");

                entity.Property(e => e.IdMetodoPago).HasColumnName("idMetodoPago");

                entity.Property(e => e.IdMonto).HasColumnName("idMonto");

                entity.Property(e => e.IdPrestamista).HasColumnName("idPrestamista");

                entity.Property(e => e.IdPrestatario).HasColumnName("idPrestatario");

                entity.Property(e => e.NumeroCuenta)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("numeroCuenta");

                entity.Property(e => e.PagoDiario)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("pagoDiario");

                entity.Property(e => e.Zona)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("zona");

                entity.HasOne(d => d.IdDuracionNavigation)
                    .WithMany(p => p.Prestamos)
                    .HasForeignKey(d => d.IdDuracion)
                    .HasConstraintName("FK_DURACION_idDuracion");

                entity.HasOne(d => d.IdMetodoPagoNavigation)
                    .WithMany(p => p.Prestamos)
                    .HasForeignKey(d => d.IdMetodoPago)
                    .HasConstraintName("FK_PRESTAMO_idMetodoPago");

                entity.HasOne(d => d.IdMontoNavigation)
                    .WithMany(p => p.Prestamos)
                    .HasForeignKey(d => d.IdMonto)
                    .HasConstraintName("FK_MONTO_idMonto");

                entity.HasOne(d => d.IdPrestamistaNavigation)
                    .WithMany(p => p.PrestamoIdPrestamistaNavigations)
                    .HasForeignKey(d => d.IdPrestamista)
                    .HasConstraintName("FK_PRESTAMO_idPrestamista");

                entity.HasOne(d => d.IdPrestatarioNavigation)
                    .WithMany(p => p.PrestamoIdPrestatarioNavigations)
                    .HasForeignKey(d => d.IdPrestatario)
                    .HasConstraintName("FK_PRESTAMO_idPrestatario");
            });

            modelBuilder.Entity<Rol>(entity =>
            {
                entity.ToTable("ROL");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("nombre");
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("USUARIO");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ConfContrasena)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("conf_contrasena");

                entity.Property(e => e.Contrasena)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("contrasena");

                entity.Property(e => e.Correo)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("correo");

                entity.Property(e => e.Direccion)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("direccion");

                entity.Property(e => e.Edad)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("edad");

                entity.Property(e => e.IdRol).HasColumnName("idRol");

                entity.Property(e => e.IdUsuarioCreador).HasColumnName("idUsuarioCreador");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("nombre");

                entity.HasOne(d => d.IdRolNavigation)
                    .WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.IdRol)
                    .HasConstraintName("FK_USUARIO_idRol");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
