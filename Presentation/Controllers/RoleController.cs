using Application.DTO.Role;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole([FromBody] CreateRoleDto createRoleDto)
        {
            var role = await _roleService.CreateRoleAsync(createRoleDto);
            return CreatedAtAction(nameof(GetRoleById), new { id = role.Id }, role);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRoleById([FromRoute] int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            return Ok(role);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<RoleDto>> GetRoleByName([FromRoute] string name)
        {
            var role = await _roleService.GetRoleByNameAsync(name);
            return Ok(role);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRole([FromRoute] int id)
        {
            await _roleService.DeleteRoleByIdAsync(id);
            return Ok();
        }
    }
}
