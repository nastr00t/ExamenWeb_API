using ExamenWeb_API.Data;
using ExamenWeb_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExamenWeb_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluadosController : ControllerBase
    {
        
        private readonly Examenes_DBContext _context;
        private IConfiguration _config;

        public EvaluadosController(Examenes_DBContext context, IConfiguration config)
        {
            _config = config;
            _context = context;
        }
        
        // GET: api/Evaluados
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Evaluados>>> List()
        {
            return await _context.Evaluados.ToListAsync();
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

            return evaluado;
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

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"].ToString() ?? ""));
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

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"].ToString() ?? ""));
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
            string? documento_Identidad = claim.FirstOrDefault(x => x.Type == "documento_Identidad").Value.ToString();

            var evaluado = await _context.Evaluados.Include(ev => ev.Intentos).FirstOrDefaultAsync(e => e.numero_identificacion == documento_Identidad);

            if (evaluado == null) {
                return BadRequest(new { status = "failed", message = "Evaluado no activo" });
            }
            var examen = await _context.Examen.Include(e=>e.Examen_Categorias).FirstOrDefaultAsync(e=> e.id_examen == idExamen);
            if (examen == null) {
                return BadRequest(new { status = "failed", message = "Exámen no válido" });
            }

            int preguntas = examen.cantidad_preguntas;
            Intentos intento = new Intentos() { id_examen = idExamen, id_evaluado = evaluado.id_evaluado,fecha_intento = DateTime.Now };

            foreach (Examen_Categorias categoria in examen.Examen_Categorias)
            {
                if (int.TryParse((Convert.ToDouble(preguntas) * (categoria.peso_minimo / 100)).ToString(), out int preguntasCAT))
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


        private bool EvaluadoExists(int id)
        {
            return _context.Evaluados.Any(e => e.id_evaluado == id);
        }
 
    }
}
