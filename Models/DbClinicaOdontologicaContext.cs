using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AppClinicaOdontologica.Models;

public partial class DbClinicaOdontologicaContext : DbContext
{
    public DbClinicaOdontologicaContext()
    {
    }

    public DbClinicaOdontologicaContext(DbContextOptions<DbClinicaOdontologicaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Consultum> Consulta { get; set; }

    public virtual DbSet<Paciente> Pacientes { get; set; }

    public virtual DbSet<Procedimento> Procedimentos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConexaoSqlServer");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Consultum>(entity =>
        {
            entity.HasKey(e => e.IdConsulta).HasName("PK__Consulta__9B2AD1D8AB2F967A");

            entity.Property(e => e.DataConsulta).HasColumnType("datetime");
            entity.Property(e => e.Observacoes)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.Consulta)
                .HasForeignKey(d => d.IdPaciente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consulta_Paciente");

            entity.HasOne(d => d.IdProcedimentoNavigation).WithMany(p => p.Consulta)
                .HasForeignKey(d => d.IdProcedimento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Consulta_Procedimento");
        });

        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.HasKey(e => e.IdPaciente).HasName("PK__Paciente__C93DB49B120DBE60");

            entity.ToTable("Paciente");

            entity.Property(e => e.Cpf)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("CPF");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Telefone)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Procedimento>(entity =>
        {
            entity.HasKey(e => e.IdProcedimento).HasName("PK__Procedim__8DD91F10691F6B14");

            entity.ToTable("Procedimento");

            entity.Property(e => e.Descricao)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.Valor).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.Procedimentos)
                .HasForeignKey(d => d.IdPaciente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Procedimento_Paciente");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__5B65BF97460EEF66");

            entity.ToTable("Usuario");

            entity.HasIndex(e => e.Email, "UQ__Usuario__A9D1053416DA31DD").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SenhaHash)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
