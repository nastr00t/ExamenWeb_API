using System;
using System.Collections.Generic;

namespace ExamenWeb_API.Models;

public partial class Respuestas_Intento
{
    public int id_respuesta_intento { get; set; }

    public int? id_intento { get; set; }

    public int? id_pregunta { get; set; }

    public int? id_respuesta { get; set; }

    public int? tiempo_respuesta { get; set; }

    public virtual Intentos? id_intentoNavigation { get; set; }

    public virtual Preguntas? id_preguntaNavigation { get; set; }

    public virtual Respuestas? id_respuestaNavigation { get; set; }
}
