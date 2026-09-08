using dotnetapp.DTOs;
using dotnetapp.Services;
using Microsoft.AspNetCore.Mvc;

namespace dotnetapp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthenticationService _service;

        public AuthController(
            AuthenticationService service)
        {
            _service = service;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result =
                await _service.RegisterAsync(dto);

            if (!result.Success)
                return BadRequest(new
                {
                    message = result.Message
                });

            return Ok(new
            {
                message = result.Message,
                token = result.Token
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result =
                await _service.LoginAsync(dto);

            if (!result.Success)
                return Unauthorized(new
                {
                    message = result.Message
                });

            return Ok(new
            {
                message = result.Message,
                token = result.Token
            });
        }
    }
}