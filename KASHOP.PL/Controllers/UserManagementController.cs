using KASHOP.BL.Service;
using KASHOP.DAL.DTO.Request;
using KASHOP.PL.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace KASHOP.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public UserManagementController(IUserManagementService userManagementService, IStringLocalizer<SharedResources> localizer)
        {
            _userManagementService = userManagementService;
            _localizer = localizer;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManagementService.GetAllUsers();
            return Ok(users);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser([FromRoute] string userId)
        {
            var user = await _userManagementService.GetUser(userId);
            return Ok(new { user });
        }

        [HttpPatch("{userId}/role")]
        public async Task<IActionResult> ChangeRole(string userId, [FromBody] ChangeRoleRequest request)
        {
            var result = await _userManagementService.ChangeRole(userId, request.NewRole);

            if (!result) return BadRequest();

            return Ok(result);
        }

        [HttpPatch("{userId}/toggle-block")]
        public async Task<IActionResult> ToggleBlock(string userId, [FromBody] ChangeRoleRequest request)
        {
            var result = await _userManagementService.toggleBlockUser(userId);

            if (!result) return BadRequest();

            return Ok(result);
        }
    }
}
