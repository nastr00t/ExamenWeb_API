using System;
using System.Collections.Generic;

namespace ExamenWeb_API.Models;

public partial class Preguntas
{
    public int id_pregunta { get; set; }

    public int? id_categoria { get; set; }

    public string texto_pregunta { get; set; } = null!;

    public int? id_usuario_creador { get; set; }

    public DateTime? fecha_creacion { get; set; }

    public virtual ICollection<Respuestas> Respuestas { get; set; } = new List<Respuestas>();

    public virtual ICollection<Respuestas_Intento> Respuestas_Intento { get; set; } = new List<Respuestas_Intento>();

    public virtual Categorias? id_categoriaNavigation { get; set; }

    public virtual Usuarios? id_usuario_creadorNavigation { get; set; }

    public virtual ICollection<Examen> id_examen { get; set; } = new List<Examen>();
}
