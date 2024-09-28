using ExamenWeb_API.Data;
using ExamenWeb_API.Models;
using ExamenWeb_API.Clases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ExamenWeb_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExamenesController : ControllerBase
    {
        private readonly examendbContext _context;
        public ExamenesController(examendbContext context)
        {
            _context = context;
        }
        
        // GET: api/Examenes
        [HttpGet("VerExamenes")]        
        public async Task<ActionResult<IEnumerable<Examenes>>> GetExamen()
        {
            return await _context.Examenes.Where(e=> e.estado==true).ToListAsync();
        }

        // GET: api/Examenes/   

        [HttpGet("{id}")]
        public async Task<ActionResult<Examenes>> GetExamen(int id)
        {
            var examen = await _context.Examenes.FindAsync(id);

            if (examen == null)
            {
                return NotFound();
            }

            return  examen;
        }

        // POST: api/Examenes
      
        [HttpPost("IngresarExamen")]
        
        public async Task<ActionResult<Examenes>>PostExamen(Examenes examen)
        {
            _context.Examenes.Add(examen);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetExamen", new { id = examen.id_examen }, examen);

        }

        
        // PUT: api/Examenes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExamen(int  id, Examenes examen)
        {
            if (id != examen.id_examen)
            {
                return BadRequest();
            }

            _context.Entry(examen).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)

            {
                if (!ExamenExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();

        }


        [HttpPost("IngresarCategoria")]
        public async Task<ActionResult<Categorias>> IngresarCategoriaAsync(Categorias categoria)
        {
            if (categoria != null)
            {
                _context.Categorias.Add(categoria);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetCategorias", new { id = categoria.id_categoria }, categoria);
            }
            return BadRequest();
        }

        [HttpGet("GetCategorias")]
        public async Task<ActionResult<Categorias>> GetCategorias(int id)
        {
            var Categoria = await _context.Categorias.FindAsync(id);

            if (Categoria == null)
            {
                return NotFound();
            }

            return Categoria;
        }

        [HttpPost("IngresarPregunta")]
        public async Task<ActionResult<Categorias>> IngresarPreguntaAsync(Preguntas pregunta)
        {
            if (pregunta != null)
            {
                if (pregunta.Respuestas.Count==0)
                    return StatusCode(403, new { status = "failed", message = "Debe registra las repuestas de la pregunta" });

                _context.Preguntas.Add(pregunta);
                await _context.SaveChangesAsync();
                 

                return CreatedAtAction("GetPregunta", new { id = pregunta.id_pregunta}, pregunta);
            }
            return BadRequest();
        }

        [HttpGet("ListarPreguntas")]
        public async Task<ActionResult<List<Preguntas>>> GetPreguntas()
        {
            var preguntas = await _context.Preguntas.Include(r=>r.Respuestas).ToListAsync();
            if (preguntas == null)
            {
                return NotFound();
            }
            return preguntas;
        }

        [HttpGet("GetPregunta/{id}")]
        public async Task<ActionResult<Preguntas>> GetPregunta(int id)
        {
            var pregunta = await _context.Preguntas.Include(r => r.Respuestas).FirstAsync(p=>p.id_pregunta == id);
            if (pregunta == null)
            {
                return NotFound();
            }
            return pregunta;
        }

        [HttpPost("CargarPlantilla")]
        public async Task<IActionResult> CargarPlantillaAsync(IFormFile file)
        {
            // Cast to ClaimsIdentity.
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            // Gets list of claims.
            IEnumerable<Claim> claim = identity.Claims;
            // Gets name from claims. Generally it's an email address.
            int user = Convert.ToInt32(claim.FirstOrDefault(x => x.Type == "id_usuario").Value);


            string pathToSave = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(pathToSave))
                Directory.CreateDirectory(pathToSave);

            string fullPath = Path.Combine(pathToSave, DateTime.Now.Ticks.ToString() + "_" + file.FileName);
            using FileStream stream = new(fullPath, FileMode.Create);
            file.CopyTo(stream);


            bool process = await new ProcesarArchivo(_context).ProcesarExcel(stream, user);
            if (!process)
            {
                stream.Close();
                return await Task.FromResult<IActionResult>(BadRequest());
            }
            else
            {
                stream.Close();
                return await Task.FromResult<IActionResult>(Ok(new { status = "success", message = "Archivo cargado y procesado exitosamente." }));
            }
        }

        private bool ExamenExists(int id)
        {
            return _context.Examenes.Any(e => e.id_examen == id);
        }
    }
}
