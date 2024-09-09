using ExamenWeb_API.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamenWeb_API.Data;

public partial class Examenes_DBContext : DbContext
{
    public Examenes_DBContext()
    {
    }

    public Examenes_DBContext(DbContextOptions<Examenes_DBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categorias> Categorias { get; set; }

    public virtual DbSet<Certificados> Certificados { get; set; }

    public virtual DbSet<Certificados_Categoria> Certificados_Categoria { get; set; }

    public virtual DbSet<Evaluados> Evaluados { get; set; }

    public virtual DbSet<Examen> Examen { get; set; }

    public virtual DbSet<Examen_Categorias> Examen_Categorias { get; set; }

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
            entity.HasKey(e => e.id_categoria).HasName("PK__Categori__CD54BC5A155B94CC");

            entity.Property(e => e.fecha_creacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.nombre)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.id_usuario_creadorNavigation).WithMany(p => p.Categorias)
                .HasForeignKey(d => d.id_usuario_creador)
                .HasConstraintName("FK__Categoria__id_us__46E78A0C");
        });

        modelBuilder.Entity<Certificados>(entity =>
        {
            entity.HasKey(e => e.id_certificado).HasName("PK__Certific__B7A2D13B5005F766");

            entity.Property(e => e.estado).HasDefaultValue(true);
            entity.Property(e => e.fecha_emision).HasColumnType("datetime");

            entity.HasOne(d => d.id_evaluadoNavigation).WithMany(p => p.Certificados)
                .HasForeignKey(d => d.id_evaluado)
                .HasConstraintName("FK__Certifica__id_ev__5EBF139D");
        });

        modelBuilder.Entity<Certificados_Categoria>(entity =>
        {
            entity.HasKey(e => new { e.id_certificado, e.id_categoria }).HasName("PK__Certific__0B779AFE5999DD67");

            entity.HasOne(d => d.id_categoriaNavigation).WithMany(p => p.Certificados_Categoria)
                .HasForeignKey(d => d.id_categoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Certifica__id_ca__628FA481");

            entity.HasOne(d => d.id_certificadoNavigation).WithMany(p => p.Certificados_Categoria)
                .HasForeignKey(d => d.id_certificado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Certifica__id_ce__619B8048");
        });

        modelBuilder.Entity<Evaluados>(entity =>
        {
            entity.HasKey(e => e.id_evaluado).HasName("PK__Evaluado__212C44908FB596F9");

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

        modelBuilder.Entity<Examen>(entity =>
        {
            entity.HasKey(e => e.id_examen).HasName("PK__Examen__D16A231D53F18830");

            entity.Property(e => e.descripcion).HasColumnType("text");
            entity.Property(e => e.estado).HasDefaultValue(true);
            entity.Property(e => e.fecha_creacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.titulo)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.id_usuario_creadorNavigation).WithMany(p => p.Examen)
                .HasForeignKey(d => d.id_usuario_creador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Examen__id_usuar__4316F928");

            entity.HasMany(d => d.id_pregunta).WithMany(p => p.id_examen)
                .UsingEntity<Dictionary<string, object>>(
                    "Examen_Preguntas",
                    r => r.HasOne<Preguntas>().WithMany()
                        .HasForeignKey("id_pregunta")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Examen_Preguntas_Preguntas"),
                    l => l.HasOne<Examen>().WithMany()
                        .HasForeignKey("id_examen")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Examen_Preguntas_Examen"),
                    j =>
                    {
                        j.HasKey("id_examen", "id_pregunta");
                    });
        });

        modelBuilder.Entity<Examen_Categorias>(entity =>
        {
            entity.HasKey(e => new { e.id_examen, e.id_categoria }).HasName("PK__Examen_C__6DBF68D8ACABE212");

            entity.HasOne(d => d.id_categoriaNavigation).WithMany(p => p.Examen_Categorias)
                .HasForeignKey(d => d.id_categoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Examen_Ca__id_ca__52593CB8");

            entity.HasOne(d => d.id_examenNavigation).WithMany(p => p.Examen_Categorias)
                .HasForeignKey(d => d.id_examen)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Examen_Ca__id_ex__5165187F");
        });

        modelBuilder.Entity<Intentos>(entity =>
        {
            entity.HasKey(e => e.id_intento).HasName("PK__Intentos__58748C6DF021A5C7");

            entity.Property(e => e.fecha_intento).HasColumnType("datetime");

            entity.HasOne(d => d.id_evaluadoNavigation).WithMany(p => p.Intentos)
                .HasForeignKey(d => d.id_evaluado)
                .HasConstraintName("FK__Intentos__id_eva__5535A963");

            entity.HasOne(d => d.id_examenNavigation).WithMany(p => p.Intentos)
                .HasForeignKey(d => d.id_examen)
                .HasConstraintName("FK__Intentos__id_exa__5629CD9C");
        });

        modelBuilder.Entity<Preguntas>(entity =>
        {
            entity.HasKey(e => e.id_pregunta).HasName("PK__Pregunta__6867FFA448229A22");

            entity.Property(e => e.fecha_creacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.texto_pregunta).HasColumnType("text");

            entity.HasOne(d => d.id_categoriaNavigation).WithMany(p => p.Preguntas)
                .HasForeignKey(d => d.id_categoria)
                .HasConstraintName("FK__Preguntas__id_ca__4AB81AF0");

            entity.HasOne(d => d.id_usuario_creadorNavigation).WithMany(p => p.Preguntas)
                .HasForeignKey(d => d.id_usuario_creador)
                .HasConstraintName("FK__Preguntas__id_us__4BAC3F29");
        });

        modelBuilder.Entity<Respuestas>(entity =>
        {
            entity.HasKey(e => e.id_respuesta).HasName("PK__Respuest__14E555893F9FB780");

            entity.Property(e => e.texto_respuesta).HasColumnType("text");

            entity.HasOne(d => d.id_preguntaNavigation).WithMany(p => p.Respuestas)
                .HasForeignKey(d => d.id_pregunta)
                .HasConstraintName("FK__Respuesta__id");
        });

        modelBuilder.Entity<Respuestas_Intento>(entity =>
        {
            entity.HasKey(e => e.id_respuesta_intento).HasName("PK__Respuest__C3EBEE056ABF7D05");

            entity.HasOne(d => d.id_intentoNavigation).WithMany(p => p.Respuestas_Intento)
                .HasForeignKey(d => d.id_intento)
                .HasConstraintName("FK__Respuesta__id_in__59063A47");

            entity.HasOne(d => d.id_preguntaNavigation).WithMany(p => p.Respuestas_Intento)
                .HasForeignKey(d => d.id_pregunta)
                .HasConstraintName("FK__Respuesta__id_pr__59FA5E80");

            entity.HasOne(d => d.id_respuestaNavigation).WithMany(p => p.Respuestas_Intento)
                .HasForeignKey(d => d.id_respuesta)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Respuesta__id_re");
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(e => e.id_usuario).HasName("PK__Usuarios__4E3E04ADE3F28C9C");

            entity.HasIndex(e => e.usuario, "UQ__Usuarios__9AFF8FC62879E3CF").IsUnique();

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
