using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sisNet.models;
using sisNet.Repository;
using sisNet.Services;

namespace sisNet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly MockUserRepository _userRepository;
        private readonly AuthService _authService;

        public ClientesController(MockUserRepository userRepository, AuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Comercial,Backoffice")]
        public IActionResult GetCliente(int id)
        {
            var cliente = _userRepository.GetById(id);
            if (cliente == null || cliente.Role != UserRole.Cliente)
                return NotFound(new { message = "Cliente no encontrado" });
            return Ok(cliente);
        }

        [HttpGet("{id}/token")]
        [Authorize(Roles = "Comercial,Backoffice")]
        public IActionResult GetClienteToken(int id)
        {
            var cliente = _userRepository.GetById(id);
            if (cliente == null || cliente.Role != UserRole.Cliente)
                return NotFound(new { message = "Cliente no encontrado" });

            if (cliente is User userObj)
            {
                var token = _authService.GenerateTokenForUser(userObj, User);
                return Ok(new { token });
            }
            else
            {
                return StatusCode(500, new { message = "No se pudo generar el token para el cliente (tipo inesperado)" });
            }
        }

        [HttpPost("restore-impersonator")]
        [Authorize]
        public IActionResult RestoreImpersonator()
        {
            var originalUserId = User.FindFirst("original_user_id")?.Value;
            var originalUserRole = User.FindFirst("original_user_role")?.Value;
            if (string.IsNullOrEmpty(originalUserId) || string.IsNullOrEmpty(originalUserRole))
            {
                return BadRequest(new { message = "No se encontraron datos de usuario original en el token." });
            }

            // Buscar el usuario original por ID
            if (!int.TryParse(originalUserId, out int id))
                return BadRequest(new { message = "ID de usuario original inválido." });

            var usuarioOriginal = _userRepository.GetById(id);
            if (usuarioOriginal is not User userObj)
                return NotFound(new { message = "Usuario original no encontrado." });

            // Generar un nuevo token para el usuario original (sin claims de suplantación)
            var token = _authService.GenerateTokenForUser(userObj);
            return Ok(new { token });
        }
    }
}
