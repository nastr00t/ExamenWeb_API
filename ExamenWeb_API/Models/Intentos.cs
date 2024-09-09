using System;
using System.Collections.Generic;

namespace ExamenWeb_API.Models;

public partial class Intentos
{
    public int id_intento { get; set; }

    public int? id_evaluado { get; set; }

    public int? id_examen { get; set; }

    public DateTime fecha_intento { get; set; }

    public double? calificacion { get; set; }

    public virtual ICollection<Respuestas_Intento> Respuestas_Intento { get; set; } = new List<Respuestas_Intento>();

    public virtual Evaluados? id_evaluadoNavigation { get; set; }

    public virtual Examen? id_examenNavigation { get; set; }
}
