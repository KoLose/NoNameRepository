using Microsoft.AspNetCore.Mvc;
using WorldSkillsAPI.Interfaces;
using WorldSkillsAPI.Requests;

namespace WorldSkillsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReminderController : ControllerBase
{
    private readonly IReminderService _reminderService;

    public ReminderController(IReminderService reminderService)
    {
        _reminderService = reminderService;
    }

    [HttpPost("course/{courseId:int}/send")]
    [RoleAuthorized(2, 3)]
    public async Task<IActionResult> SendCourseReminder(
        int courseId,
        [FromBody] CourseReminderRequest request,
        CancellationToken cancellationToken)
    {
        var token = Request.Headers["Authorization"].FirstOrDefault() ?? "";
        return await _reminderService.SendCourseReminderAsync(
            courseId,
            request,
            token,
            cancellationToken);
    }
}
