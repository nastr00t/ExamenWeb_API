using System;
using System.Collections.Generic;

namespace ExamenWeb_API.Models;

public partial class Certificados
{
    public int id_certificado { get; set; }

    public int? id_evaluado { get; set; }

    public DateTime fecha_emision { get; set; }

    public bool estado { get; set; }

    public virtual ICollection<Certificados_Categoria> Certificados_Categoria { get; set; } = new List<Certificados_Categoria>();

    public virtual Evaluados? id_evaluadoNavigation { get; set; }
}
