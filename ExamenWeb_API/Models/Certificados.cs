using System;
using System.Collections.Generic;

namespace ExamenWeb_API.Models;

public partial class Certificados
{
    public int id_certificado { get; set; }

    public int? id_evaluado { get; set; }

    public DateTime fecha_emision { get; set; }

    public bool estado { get; set; }

    public virtual ICollection<Categorias_Examen> Categorias_Examen { get; set; } = new List<Categorias_Examen>();

    public virtual Evaluados? id_evaluadoNavigation { get; set; }
}
