using System;
using System.Collections.Generic;

namespace ExamenWeb_API.Models;

public partial class Evaluados
{
    public int id_evaluado { get; set; }

    public string numero_identificacion { get; set; } = null!;

    public string nombre { get; set; } = null!;

    public string apellidos { get; set; } = null!;

    public string cargo { get; set; } = null!;

    public string ciudad { get; set; } = null!;

    public string correo { get; set; } = null!;

    public bool estado { get; set; }

    public DateTime? fecha_creacion { get; set; }

    public virtual ICollection<Certificados> Certificados { get; set; } = new List<Certificados>();

    public virtual ICollection<Intentos> Intentos { get; set; } = new List<Intentos>();
}
