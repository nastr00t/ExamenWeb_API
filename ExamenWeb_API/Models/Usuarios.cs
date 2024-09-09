using System;
using System.Collections.Generic;

namespace ExamenWeb_API.Models;

public partial class Usuarios
{
    public int id_usuario { get; set; }

    public string usuario { get; set; } = null!;

    public string password { get; set; } = null!;

    public string tipo_usuario { get; set; } = null!;

    public string nombre { get; set; } = null!;

    public string apellidos { get; set; } = null!;

    public string correo { get; set; } = null!;

    public bool estado { get; set; }

    public DateTime? fecha_creacion { get; set; }

    public virtual ICollection<Categorias> Categorias { get; set; } = new List<Categorias>();

    public virtual ICollection<Examen> Examen { get; set; } = new List<Examen>();

    public virtual ICollection<Preguntas> Preguntas { get; set; } = new List<Preguntas>();
}
