using Application.DTO.User;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Register(CreateUserDto dto)
        {
            try
            {
                var user = await _userService.CreateUserAsync(dto);

                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById([FromRoute] int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);

                return Ok(user);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetUserByEmail([FromBody] string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);

                return Ok(user);
            }
            catch (ArgumentException)
            {
                return NotFound(email);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByName([FromQuery] string name)
        {
            try
            {
                var users = await _userService.GetUsersByNameAsync(name);

                return Ok(users);
            }
            catch (ArgumentException)
            {
                return NotFound(name);
            }
        }
    }
}
