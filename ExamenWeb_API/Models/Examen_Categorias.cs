using System;
using System.Collections.Generic;

namespace ExamenWeb_API.Models;

public partial class Examen_Categorias
{
    public int id_examen { get; set; }

    public int id_categoria { get; set; }

    public double? peso_minimo { get; set; }

    public double? peso_maximo { get; set; }

    public virtual Categorias id_categoriaNavigation { get; set; } = null!;

    public virtual Examen id_examenNavigation { get; set; } = null!;
}
