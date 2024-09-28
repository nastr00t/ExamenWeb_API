using System;
using System.Collections.Generic;
using ExamenWeb_API.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamenWeb_API.Data;

public partial class examendbContext : DbContext
{
    public examendbContext()
    {
    }

    public examendbContext(DbContextOptions<examendbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categorias> Categorias { get; set; }

    public virtual DbSet<Categorias_Examen> Categorias_Examen { get; set; }

    public virtual DbSet<Certificados> Certificados { get; set; }

    public virtual DbSet<Evaluados> Evaluados { get; set; }

    public virtual DbSet<Examenes> Examenes { get; set; }

    public virtual DbSet<Intentos> Intentos { get; set; }

    public virtual DbSet<Preguntas> Preguntas { get; set; }

    public virtual DbSet<Respuestas> Respuestas { get; set; }

    public virtual DbSet<Respuestas_Intento> Respuestas_Intento { get; set; }

    public virtual DbSet<Usuarios> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()

     .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
     .AddJsonFile("appsettings.json")
     .Build();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("ExamenConn"));


    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categorias>(entity =>
        {
            entity.HasKey(e => e.id_categoria).HasName("PK__Categori__CD54BC5AFEB45F17");

            entity.Property(e => e.fecha_creacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.nombre)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.id_usuarioNavigation).WithMany(p => p.Categorias)
                .HasForeignKey(d => d.id_usuario)
                .HasConstraintName("FK_Categorias_Usuarios");
        });

        modelBuilder.Entity<Categorias_Examen>(entity =>
        {
            entity.HasKey(e => new { e.id_examen, e.id_categoria }).HasName("PK__Categori__6DBF68D8602FE59D");

            entity.HasOne(d => d.id_categoriaNavigation).WithMany(p => p.Categorias_Examen)
                .HasForeignKey(d => d.id_categoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Categorias_Examen_Categorias");

            entity.HasOne(d => d.id_examenNavigation).WithMany(p => p.Categorias_Examen)
                .HasForeignKey(d => d.id_examen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Categorias_Examen_Certificados");

            entity.HasOne(d => d.examen).WithMany(p => p.Categorias_Examen)
                .HasForeignKey(d => d.id_examen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Categorias_Examen_Examenes");
        });

        modelBuilder.Entity<Certificados>(entity =>
        {
            entity.HasKey(e => e.id_certificado).HasName("PK__Certific__B7A2D13B2FD5FBD4");

            entity.Property(e => e.estado).HasDefaultValue(true);
            entity.Property(e => e.fecha_emision).HasColumnType("datetime");

            entity.HasOne(d => d.id_evaluadoNavigation).WithMany(p => p.Certificados)
                .HasForeignKey(d => d.id_evaluado)
                .HasConstraintName("FK_Certificados_Evaluados");
        });

        modelBuilder.Entity<Evaluados>(entity =>
        {
            entity.HasKey(e => e.id_evaluado).HasName("PK__Evaluado__212C4490DC6564EF");

            entity.Property(e => e.apellidos)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.cargo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.ciudad)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.correo)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.estado).HasDefaultValue(true);
            entity.Property(e => e.fecha_creacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.numero_identificacion)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Examenes>(entity =>
        {
            entity.HasKey(e => e.id_examen).HasName("PK__Examenes__D16A231D8F8DAE98");

            entity.Property(e => e.descripcion).HasColumnType("text");
            entity.Property(e => e.estado).HasDefaultValue(true);
            entity.Property(e => e.fecha_creacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.titulo)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.id_usuarioNavigation).WithMany(p => p.Examenes)
                .HasForeignKey(d => d.id_usuario)
                .HasConstraintName("FK_Examenes_Usuarios");
        });

        modelBuilder.Entity<Intentos>(entity =>
        {
            entity.HasKey(e => e.id_intento).HasName("PK__Intentos__58748C6DEBADD4F3");

            entity.Property(e => e.fecha_intento).HasColumnType("datetime");

            entity.HasOne(d => d.id_evaluadoNavigation).WithMany(p => p.Intentos)
                .HasForeignKey(d => d.id_evaluado)
                .HasConstraintName("FK_Intentos_Evaluados");

            entity.HasOne(d => d.id_examenNavigation).WithMany(p => p.Intentos)
                .HasForeignKey(d => d.id_examen)
                .HasConstraintName("FK_Intentos_Examenes");
        });

        modelBuilder.Entity<Preguntas>(entity =>
        {
            entity.HasKey(e => e.id_pregunta).HasName("PK__Pregunta__6867FFA4B9726640");

            entity.Property(e => e.fecha_creacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.texto_pregunta).HasColumnType("text");

            entity.HasOne(d => d.id_categoriaNavigation).WithMany(p => p.Preguntas)
                .HasForeignKey(d => d.id_categoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Preguntas_Categorias");

            entity.HasOne(d => d.id_usuarioNavigation).WithMany(p => p.Preguntas)
                .HasForeignKey(d => d.id_usuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Preguntas_Usuarios");
        });

        modelBuilder.Entity<Respuestas>(entity =>
        {
            entity.HasKey(e => new { e.id_respuesta, e.id_pregunta });

            entity.Property(e => e.id_respuesta).ValueGeneratedOnAdd();
            entity.Property(e => e.texto_respuesta).HasColumnType("text");

            entity.HasOne(d => d.id_preguntaNavigation).WithMany(p => p.Respuestas)
                .HasForeignKey(d => d.id_pregunta)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Respuestas_Preguntas");
        });

        modelBuilder.Entity<Respuestas_Intento>(entity =>
        {
            entity.HasKey(e => e.id_respuesta_intento).HasName("PK__Respuest__C3EBEE054F2708C2");

            entity.HasOne(d => d.id_intentoNavigation).WithMany(p => p.Respuestas_Intento)
                .HasForeignKey(d => d.id_intento)
                .HasConstraintName("FK_Respuestas_Intento_Intentos");

            entity.HasOne(d => d.Respuestas).WithMany(p => p.Respuestas_Intento)
                .HasForeignKey(d => new { d.id_respuesta, d.id_pregunta })
                .HasConstraintName("FK_Respuestas_Intento_Respuestas");
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(e => e.id_usuario).HasName("PK__Usuarios__4E3E04ADBAC8DEDF");

            entity.Property(e => e.apellidos)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.correo)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.estado).HasDefaultValue(true);
            entity.Property(e => e.fecha_creacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.tipo_usuario)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("administrador");
            entity.Property(e => e.usuario)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
