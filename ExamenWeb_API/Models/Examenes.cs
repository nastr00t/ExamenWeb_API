using System;
using System.Collections.Generic;

namespace ExamenWeb_API.Models;

public partial class Examenes
{
    public int id_examen { get; set; }

    public string titulo { get; set; } = null!;

    public string descripcion { get; set; } = null!;

    public DateOnly fecha_limite { get; set; }

    public int cantidad_preguntas { get; set; }

    public bool estado { get; set; }

    public int? id_usuario { get; set; }

    public DateTime fecha_creacion { get; set; }

    public virtual ICollection<Categorias_Examen> Categorias_Examen { get; set; } = new List<Categorias_Examen>();

    public virtual ICollection<Intentos> Intentos { get; set; } = new List<Intentos>();

    public virtual Usuarios? id_usuarioNavigation { get; set; }
}
