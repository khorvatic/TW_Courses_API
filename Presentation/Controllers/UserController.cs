using Application.DTO.User;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using System.Security.Claims;

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
            var user = await _userService.CreateUserAsync(dto);

            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById([FromRoute] int id)
        {
            var userId = User.GetUserId();
            if (userId != id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("by-email")]
        public async Task<ActionResult<UserDto>> GetUserByEmail([FromQuery] string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("first-name")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByName([FromQuery] string name)
        {
            var users = await _userService.GetUsersByNameAsync(name);
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("last-name")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersBySurname([FromQuery] string surname)
        {
            var users = await _userService.GetUsersBySurnameAsync(surname);
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("full-name")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsersByFullName([FromQuery] string name, [FromQuery] string surname)
        {
            var users = await _userService.GetUsersByFullNameAsync(name, surname);
            return Ok(users);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser([FromRoute] int id)
        {
            var userId = User.GetUserId();
            if (userId != id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            
            await _userService.DeleteUserAsync(id);
            return Ok();
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> UpdateUser([FromRoute] int id, CreateUserDto user)
        {
            var userId = User.GetUserId();
            if (userId != id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return Ok(await _userService.UpdateUserAsync(id, user));
        }
    }
}
