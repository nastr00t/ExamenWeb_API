using ExamenWeb_API.Data;
using ExamenWeb_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExamenWeb_API.Controllers
{
    [EnableCors()]
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluadosController : ControllerBase
    {
        
        private readonly examendbContext _context;
        private IConfiguration _config;

        public EvaluadosController(examendbContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
        }
        
        // GET: api/Evaluados
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Evaluados>>> List()
        {
            List<Evaluados> evaluados = await _context.Evaluados.ToListAsync();
            return Ok(new { status = "success", message = "Examen Registrado", evaluados });
        }

        // GET: api/Evaluados/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Evaluados>> Details(int id)
        {
            var evaluado = await _context.Evaluados
                .FindAsync(id);

            if (evaluado == null)
            {
                return NotFound();
            }
            return Ok(new { status = "success", evaluado });
        }

        [HttpPost("Register")]
        public async Task<ActionResult<Evaluados>> Register([FromBody] Evaluados evaluado)
        {
            if (!_context.Evaluados.Any(e => e.numero_identificacion == evaluado.numero_identificacion))
            {
                _context.Evaluados.Add(evaluado);
                await _context.SaveChangesAsync();

                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Name, evaluado.nombre),
                    new Claim(JwtRegisteredClaimNames.NameId, evaluado.id_evaluado.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, evaluado.correo),
                    new Claim("Apellidos", evaluado.apellidos),
                    new Claim("documento_Identidad", evaluado.numero_identificacion),
                    new Claim(JwtRegisteredClaimNames.Sub, evaluado.ciudad),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? ""));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Issuer"],
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds);

                return Ok(new { status = "success", id = evaluado.id_evaluado, token = new JwtSecurityTokenHandler().WriteToken(token), evaluado = evaluado });
            }
            else
            {

                Evaluados evaluadoEncontrado = _context.Evaluados.First(e => e.numero_identificacion == evaluado.numero_identificacion);
                evaluadoEncontrado.nombre = evaluado.nombre;
                evaluadoEncontrado.apellidos = evaluado.apellidos;
                evaluadoEncontrado.correo = evaluado.correo;
                evaluadoEncontrado.cargo = evaluado.cargo;
                evaluadoEncontrado.ciudad = evaluado.ciudad;
                _context.Entry(evaluadoEncontrado).State = EntityState.Modified;
                await _context.SaveChangesAsync();


                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Name, evaluado.nombre),
                    new Claim(JwtRegisteredClaimNames.NameId, evaluado.id_evaluado.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, evaluado.correo),
                    new Claim("Apellidos", evaluado.apellidos),
                    new Claim("documento_Identidad", evaluado.numero_identificacion),
                    new Claim(JwtRegisteredClaimNames.Sub, evaluado.ciudad),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? ""));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Issuer"],
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds);
                return Ok(new { status = "success", id = evaluadoEncontrado.id_evaluado, token = new JwtSecurityTokenHandler().WriteToken(token), evaluado = evaluadoEncontrado });

            }
        }
       
        [HttpPost("CreateExamenEvaluado")]
        public async Task<ActionResult> CreateExamenEvaluado( int idExamen )
        {
            // Cast to ClaimsIdentity.
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            // Gets list of claims.
            IEnumerable<Claim> claim = identity.Claims;
            // Gets name from claims. Generally it's an email address.
            string? documento_Identidad = claim.First(x => x.Type == "documento_Identidad").Value.ToString();

            var evaluado = await _context.Evaluados.Include(ev => ev.Intentos).ThenInclude(i=>i.Respuestas_Intento).FirstOrDefaultAsync(e => e.numero_identificacion == documento_Identidad);

            if (evaluado == null) {
                return BadRequest(new { status = "failed", message = "Evaluado no activo" });
            }
            var examen = await _context.Examenes.Include(e=>e.Categorias_Examen).FirstOrDefaultAsync(e=> e.id_examen == idExamen);
            if (examen == null) {
                return BadRequest(new { status = "failed", message = "Exámen no válido" });
            }
            if (_context.Intentos.Any(i => i.id_evaluado == evaluado.id_evaluado && i.id_examen == idExamen && i.calificacion==0))
            {
                return Ok(new { status = "success", message = "Examen Registrado", preguntas = evaluado.Intentos });
            }
            int preguntas = examen.cantidad_preguntas;
            Intentos intento = new Intentos() { id_examen = idExamen, id_evaluado = evaluado.id_evaluado,fecha_intento = DateTime.Now, calificacion=0 };

            foreach (Categorias_Examen categoria in examen.Categorias_Examen)
            {
                if (int.TryParse(categoria.cantidad_preguntas.ToString(), out int preguntasCAT))
                {
                    List<Preguntas> preguntasCategoria = _context.Preguntas.Where(p=> p.id_categoria == categoria.id_categoria).OrderBy(r => Guid.NewGuid()).Take(preguntasCAT).ToList();
                    foreach (var item in preguntasCategoria)
                    {
                        intento.Respuestas_Intento.Add(new Respuestas_Intento() { id_pregunta = item.id_pregunta });
                    }
                }
            }
            evaluado.Intentos.Add(intento);
            _context.Entry(evaluado).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { status = "success", message = "Examen Registrado" ,preguntas = evaluado.Intentos});
            
        }


        [HttpPost("IngresarRespuestaPregunta")]
        public async Task<ActionResult> IngresarRespuestaPregunta([FromBody] Respuestas_Intento respuesta )
        {
            // Cast to ClaimsIdentity.
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            // Gets list of claims.
            IEnumerable<Claim> claim = identity.Claims;
            // Gets name from claims. Generally it's an email address.
            string? documento_Identidad = claim.First(x => x.Type == "documento_Identidad").Value.ToString();

            var evaluado = await _context.Evaluados.Include(ev => ev.Intentos).ThenInclude(i => i.Respuestas_Intento).FirstOrDefaultAsync(e => e.numero_identificacion == documento_Identidad);

            if (evaluado == null)
            {
                return BadRequest(new { status = "failed", message = "Evaluado no activo" });
            }
            Respuestas res = _context.Respuestas.First(r => r.id_respuesta == respuesta.id_respuesta);
            evaluado.Intentos.First(i => i.calificacion == 0).Respuestas_Intento.First(r => r.id_pregunta == respuesta.id_pregunta).id_respuesta = respuesta.id_respuesta;
            evaluado.Intentos.First(i => i.calificacion == 0).Respuestas_Intento.First(r => r.id_pregunta == respuesta.id_pregunta).tiempo_respuesta = res.es_correcta?1:0;
           
            if (evaluado.Intentos.First(i => i.calificacion == 0).Respuestas_Intento.Count(r => r.id_respuesta == null) == 0)
            { 
                List<Respuestas> respuestas =_context.Respuestas.ToList();
                foreach (Respuestas respu in respuestas)
                {
                    if (evaluado.Intentos.First(i => i.calificacion == 0).Respuestas_Intento.Any(r => r.id_respuesta == respu.id_respuesta))
                        evaluado.Intentos.First(i => i.calificacion == 0).Respuestas_Intento.First(r => r.id_pregunta == respu.id_pregunta).tiempo_respuesta = respu.es_correcta ? 1 : 0;
                }
                evaluado.Intentos.First(i => i.calificacion == 0).calificacion = evaluado.Intentos.First(i => i.calificacion == 0).Respuestas_Intento.Sum(c=> c.tiempo_respuesta);
            }
            _context.Entry(evaluado).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(new { status = "success", message = "Respuesta registrado", preguntas = evaluado.Intentos });
        }



        private bool EvaluadoExists(int id)
        {
            return _context.Evaluados.Any(e => e.id_evaluado == id);
        }
 
    }
}
