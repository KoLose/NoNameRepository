using WorldSkillsAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WorldSkillsAPI.Controllers
{
    [ApiController]
    public class DirectoryController : Controller
    {
        private readonly IDirectoryService _directoryService;

        public DirectoryController(IDirectoryService directoryService)
        {
            _directoryService = directoryService;
        }


        [HttpGet]
        [Route("Roles")]
        [RoleAuthorized(2, 3)]
        public async Task<IActionResult> Roles()
        {
            return await _directoryService.GetRolesAsync();
        }

        [HttpGet]
        [Route("ProgressStatuses")]
        [RoleAuthorized(2, 3)]
        public async Task<IActionResult> ProgressStatuses()
        {
            return await _directoryService.GetProgressStatusesAsync();
        }

        [HttpGet]
        [Route("AnswerStatuses")]
        [RoleAuthorized(2, 3)]
        public async Task<IActionResult> AnswerStatuses()
        {
            return await _directoryService.GetAnswerStatusesAsync();
        }

        [HttpGet]
        [Route("Periodicities")]
        [RoleAuthorized(2, 3)]
        public async Task<IActionResult> Periodicities()
        {
            return await _directoryService.GetPeriodicitiesAsync();
        }
    }
}