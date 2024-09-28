using System;
using System.Collections.Generic;

namespace ExamenWeb_API.Models;

public partial class Categorias
{
    public int id_categoria { get; set; }

    public string nombre { get; set; } = null!;

    public int? id_usuario { get; set; }

    public DateTime? fecha_creacion { get; set; }

    public virtual ICollection<Categorias_Examen> Categorias_Examen { get; set; } = new List<Categorias_Examen>();

    public virtual ICollection<Preguntas> Preguntas { get; set; } = new List<Preguntas>();

    public virtual Usuarios? id_usuarioNavigation { get; set; }
}
