using System;
using System.Collections.Generic;

namespace ExamenWeb_API.Models;

public partial class Categorias
{
    public int id_categoria { get; set; }

    public string nombre { get; set; } = null!;

    public int? id_usuario_creador { get; set; }

    public DateTime? fecha_creacion { get; set; }

    public virtual ICollection<Certificados_Categoria> Certificados_Categoria { get; set; } = new List<Certificados_Categoria>();

    public virtual ICollection<Examen_Categorias> Examen_Categorias { get; set; } = new List<Examen_Categorias>();

    public virtual ICollection<Preguntas> Preguntas { get; set; } = new List<Preguntas>();

    public virtual Usuarios? id_usuario_creadorNavigation { get; set; }
}
