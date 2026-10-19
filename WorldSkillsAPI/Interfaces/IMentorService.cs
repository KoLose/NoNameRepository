using Microsoft.AspNetCore.Mvc;

namespace WorldSkillsAPI.Interfaces;

public interface IMentorService
{
    Task<IActionResult> GetOwnCoursesAsync(
        string token,
        CancellationToken cancellationToken = default);

    Task<IActionResult> GetOtherCoursesAsync(
        string token,
        CancellationToken cancellationToken = default);
}
