using System;
using System.Collections.Generic;

namespace ExamenWeb_API.Models;

public partial class Examen
{
    public int id_examen { get; set; }

    public string titulo { get; set; } = null!;

    public string descripcion { get; set; } = null!;

    public DateOnly fecha_limite { get; set; }

    public int cantidad_preguntas { get; set; }

    public bool estado { get; set; }

    public int id_usuario_creador { get; set; }

    public DateTime fecha_creacion { get; set; }

    public virtual ICollection<Examen_Categorias> Examen_Categorias { get; set; } = new List<Examen_Categorias>();

    public virtual ICollection<Intentos> Intentos { get; set; } = new List<Intentos>();

    public virtual Usuarios id_usuario_creadorNavigation { get; set; } = null!;

    public virtual ICollection<Preguntas> id_pregunta { get; set; } = new List<Preguntas>();
}
