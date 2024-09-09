using System;
using System.Collections.Generic;

namespace ExamenWeb_API.Models;

public partial class Certificados_Categoria
{
    public int id_certificado { get; set; }

    public int id_categoria { get; set; }

    public double? puntaje { get; set; }

    public virtual Categorias id_categoriaNavigation { get; set; } = null!;

    public virtual Certificados id_certificadoNavigation { get; set; } = null!;
}
