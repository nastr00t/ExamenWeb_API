using System;
using System.Collections.Generic;

namespace ExamenWeb_API.Models;

public partial class Categorias_Examen
{
    public int id_categoria { get; set; }

    public int id_examen { get; set; }

    public int? cantidad_preguntas { get; set; }

    public int? porcentaje_examen { get; set; }

    public virtual Categorias id_categoriaNavigation { get; set; } = null!;

    public virtual Examenes examen { get; set; } = null!;

    public virtual Certificados id_examenNavigation { get; set; } = null!;
}
