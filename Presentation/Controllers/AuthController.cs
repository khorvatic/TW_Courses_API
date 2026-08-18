using Application.DTO.Login;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<ActionResult<string>> Login(LoginDto dto)
        {
            try
            {
                var token = await _authService.LoginAsync(dto);

                return Ok(token);
            }
            catch (ArgumentException)
            {
                return Unauthorized();
            }
        }
    }
}
