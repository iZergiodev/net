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

            // Generar token para el cliente
            var token = _authService.Authenticate(cliente.Username, cliente.Password);
            if (token == null)
                return StatusCode(500, new { message = "No se pudo generar el token para el cliente" });

            return Ok(new { token });
        }
    }
}
