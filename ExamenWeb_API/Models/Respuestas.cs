using System;
using System.Collections.Generic;

namespace ExamenWeb_API.Models;

public partial class Respuestas
{
    public int id_respuesta { get; set; }

    public int id_pregunta { get; set; }

    public string texto_respuesta { get; set; } = null!;

    public bool es_correcta { get; set; }

    public virtual ICollection<Respuestas_Intento> Respuestas_Intento { get; set; } = new List<Respuestas_Intento>();

    public virtual Preguntas? id_preguntaNavigation { get; set; } = null!;
}
