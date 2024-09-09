using ExamenWeb_API.Data;
using ExamenWeb_API.Models;
using OfficeOpenXml;

namespace ExamenWeb_API.Clases
{
    public class ProcesarArchivo
    {
        private readonly Examenes_DBContext db;
 

        public ProcesarArchivo(Examenes_DBContext context)
        {
            db = context;
        }
        public async Task<bool> ProcesarExcel(Stream stream, int user)
        {
            //LECTURA DE ARCHIVO
            List<Preguntas>preguntasList = new List<Preguntas>();
            List<Categorias> categoriasList = new List<Categorias>();
            int examen = 0;

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets["Categorias"];
                    var rowCount = worksheet.Dimension.Rows;
                    var colCount = worksheet.Dimension.Columns;

                    for (int row = 1; row <= rowCount; row++)
                    {
                        if (int.TryParse(worksheet.Cells[row, 1].Value.ToString(), out int codigoCategoria))
                        {
                            var categoria = db.Categorias.Find(codigoCategoria);
                            if (categoria == null)
                            {
                                string? v = worksheet.Cells[row, 2].Value.ToString();
                                if (v != null)
                                {
                                    Categorias cat = new Categorias() { nombre = v };
                                    db.Categorias.Add(cat);
                                    await db.SaveChangesAsync();
                                }
                            }
                        }

                        // Procesa el valor de la celda según tus necesidades

                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            //Procesar Preguntas

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets["Preguntas"];
                    var rowCount = worksheet.Dimension.Rows;
                    var colCount = worksheet.Dimension.Columns;

                    for (int row = 1; row <= rowCount; row++)
                    {
                        if (int.TryParse(worksheet.Cells[row, 1].Value.ToString(), out int codigoPregunta))
                        {
                            string? v = worksheet.Cells[row, 2].Value.ToString();
                            int.TryParse(worksheet.Cells[row, 3].Value.ToString(), out int codigoCategoria);
                            int.TryParse(worksheet.Cells[row, 4].Value.ToString(), out int codigoExamen);
                            if (v != null)
                            {
                                Preguntas pregunta  = new Preguntas() {id_pregunta = codigoPregunta,id_categoria = codigoCategoria, texto_pregunta= v, id_usuario_creador =user};
                                preguntasList.Add(pregunta);
                                examen = codigoExamen; 
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            //Procesar Respuestas

            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets["Respuestas"];
                    var rowCount = worksheet.Dimension.Rows;
                    var colCount = worksheet.Dimension.Columns;

                    for (int row = 1; row <= rowCount; row++)
                    {
                        if (int.TryParse(worksheet.Cells[row, 1].Value.ToString(), out int codigoPregunta))
                        {
                            string? v = worksheet.Cells[row, 2].Value.ToString();
                            bool r = false;
                            if (worksheet.Cells[row, 3].Value.ToString() == "1")
                                r = true;

                            if (v != null)
                            {
                                Respuestas respuesta = new Respuestas() { texto_respuesta = v, es_correcta = r };
                                preguntasList.First(p => p.id_pregunta == codigoPregunta).Respuestas.Add(respuesta);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            preguntasList.ForEach(p => { 
                p.id_pregunta = 0; 
                p.Respuestas.ToList().ForEach(r => r.id_pregunta = 0); 
            });
            db.Preguntas.AddRange(preguntasList);

           
            int resultados = await db.SaveChangesAsync();
            if (resultados > 0)
            {
                if (examen != 0)
                {
                    var catExamen = preguntasList.GroupBy(c => c.id_categoria);
                    foreach (var cat in catExamen) 
                    {
                        db.Examen_Categorias.Add(new Examen_Categorias() { id_categoria = Convert.ToInt32(cat.Key), id_examen = examen });
                    };
                    await db.SaveChangesAsync();
                }
                return true;
            }
            else
                return false;
        }
    }
}
