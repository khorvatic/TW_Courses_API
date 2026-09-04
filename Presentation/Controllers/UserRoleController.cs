using Application.DTO.UserRole;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleService _userRoleService;

        public UserRoleController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        [HttpPost]
        public async Task<ActionResult<UserRoleDto>> AssignRoleToUser([FromBody] UpdateUserRoleDto assignRoleToUserDto)
        {
            var userRole = await _userRoleService.CreateUserRoleAsync(assignRoleToUserDto);
            return CreatedAtAction(nameof(GetUserRoleByCompositeId), new { userId = userRole.UserId, roleId = userRole.RoleId }, userRole);
        }

        [HttpGet("userrole")]
        public async Task<ActionResult<UserRoleDto>> GetUserRoleByCompositeId([FromQuery] int userId, [FromQuery] int roleId)
        {
            var userRole = await _userRoleService.GetByCompositeIdAsync(userId, roleId);
            return Ok(userRole);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserRoleDto>>> GetAllUserRoles()
        {
            var userRoles = await _userRoleService.GetAllUserRolesAsync();
            return Ok(userRoles);
        }

        [HttpGet("user/{userId}/roles")]
        public async Task<ActionResult<IEnumerable<UserRoleDto>>> GetRolesByUserId([FromRoute] int userId)
        {
            var userRoles = await _userRoleService.GetUserRolesByUserIdAsync(userId);
            return Ok(userRoles);
        }

        [HttpGet("role/{roleId}/users")]
        public async Task<ActionResult<IEnumerable<UserRoleDto>>> GetUsersByRoleId([FromRoute] int roleId)
        {
            var userRoles = await _userRoleService.GetUserRolesByRoleIdAsync(roleId);
            return Ok(userRoles);
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveRoleFromUser([FromQuery] int userId, [FromQuery] int roleId)
        {
            await _userRoleService.DeleteUserRoleAsync(userId, roleId);
            return Ok();
        }
    }
}
