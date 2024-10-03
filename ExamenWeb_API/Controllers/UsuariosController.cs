using ExamenWeb_API.Data;
using ExamenWeb_API.Models;
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
    public class UsuariosController : ControllerBase
    {
        private readonly examendbContext db;
        private IConfiguration _config ;

        public UsuariosController(examendbContext context, IConfiguration config)
        {
            _config = config;
            db = context;
        }


        [HttpPost("validarUsuario")]
        public async Task<IActionResult> validarUsuarioAsync(AuthenticateRequest model)
        {
            Usuarios? user = await db.Usuarios.SingleOrDefaultAsync(x => x.usuario == model.Username);
            
            // return null if user not found
            if (user == null) return Unauthorized(new { status = "failed", message = "Usuario no registrado en el sistema" });

            if (!BCrypt.Net.BCrypt.Verify(model.Password, user.password)) return Unauthorized(new { status = "failed", message = "constraseña no valida en el sistema" });



            var claims = new[]
           {
                new Claim(JwtRegisteredClaimNames.Name, user.nombre),
                new Claim(JwtRegisteredClaimNames.NameId, user.id_usuario.ToString()),
                new Claim("id_usuario", user.id_usuario.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.correo),
                new Claim("Apellidos", user.apellidos),
                new Claim(JwtRegisteredClaimNames.Sub, user.usuario),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(s: _config["Jwt:Key"].ToString()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddDays(3),
                signingCredentials: creds);

            return Ok(new
            {
                status = "success",
                token = new JwtSecurityTokenHandler().WriteToken(token),
                usuario = new
                {
                    id_usuario = user.id_usuario,
                    usuario = user.usuario,
                    tipo_usuario = user.tipo_usuario,
                    nombre = user.nombre,
                    apellidos = user.apellidos,
                    correo = user.correo,
                    fecha_creacion = user.fecha_creacion
                }
            });
        }
        
         
        // POST: api/Usuarios
        [HttpPost("Registrar")]
        public async Task<ActionResult> PostUsuario([FromBody] Usuarios usuario)
        {
            if (usuario == null)
                return BadRequest();

            if (string.IsNullOrEmpty(usuario.nombre) || string.IsNullOrEmpty(usuario.apellidos) || string.IsNullOrEmpty(usuario.correo) || string.IsNullOrEmpty(usuario.usuario) || string.IsNullOrEmpty(usuario.password))
                return BadRequest();
            usuario.password = BCrypt.Net.BCrypt.HashPassword(usuario.password, BCrypt.Net.BCrypt.GenerateSalt());

            db.Usuarios.Add(usuario);
            await db.SaveChangesAsync();

            return Ok(new
            {
                id = usuario.id_usuario,
                usuario = usuario
            });

        }
       



        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await db.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return
     NotFound();
            }

            db.Usuarios.Remove(usuario);
            await db.SaveChangesAsync();

            return
     NoContent();
        }

    }
}
