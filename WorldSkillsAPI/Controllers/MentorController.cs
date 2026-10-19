using Microsoft.AspNetCore.Mvc;
using WorldSkillsAPI.Interfaces;

namespace WorldSkillsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[RoleAuthorized(2)]
public class MentorController : ControllerBase
{
    private readonly IMentorService _mentorService;

    public MentorController(IMentorService mentorService)
    {
        _mentorService = mentorService;
    }

    [HttpGet("courses")]
    public Task<IActionResult> GetOwnCourses(CancellationToken cancellationToken)
    {
        var token = Request.Headers["Authorization"].FirstOrDefault() ?? "";
        return _mentorService.GetOwnCoursesAsync(token, cancellationToken);
    }

    [HttpGet("other-courses")]
    public Task<IActionResult> GetOtherCourses(CancellationToken cancellationToken)
    {
        var token = Request.Headers["Authorization"].FirstOrDefault() ?? "";
        return _mentorService.GetOtherCoursesAsync(token, cancellationToken);
    }
}
