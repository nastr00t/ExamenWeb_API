using System;
using System.Collections.Generic;

namespace ExamenWeb_API.Models;

public partial class Preguntas
{
    public int id_pregunta { get; set; }

    public int id_categoria { get; set; }

    public string texto_pregunta { get; set; } = null!;

    public int id_usuario { get; set; }

    public DateTime fecha_creacion { get; set; }

    public virtual ICollection<Respuestas> Respuestas { get; set; } = new List<Respuestas>();

    public virtual Categorias id_categoriaNavigation { get; set; } = null!;

    public virtual Usuarios id_usuarioNavigation { get; set; } = null!;
}
