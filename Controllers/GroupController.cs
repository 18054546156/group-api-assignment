using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        private readonly ILogger<GroupController> _logger;

        public GroupController(IGroupService groupService, ILogger<GroupController> logger)
        {
            _groupService = groupService;
            _logger = logger;
        }

        [HttpPost("create-groups")]
        public async Task<IActionResult> CreateGroups([FromQuery] string firstName)
        {
            for (int i = 1; i <= 5; i++)
            {
                var groupName = $"{firstName}-Group{i}";
                await _groupService.CreateGroupAsync(groupName);
            }
            return Ok("Successfully created all groups");
        }

        [HttpGet]
        public async Task<IActionResult> GetGroups()
        {
            var groups = await _groupService.GetGroupsAsync();
            return Ok(groups);
        }
    }
} 