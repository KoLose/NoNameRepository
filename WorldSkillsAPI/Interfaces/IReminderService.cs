using Microsoft.AspNetCore.Mvc;
using WorldSkillsAPI.Requests;

namespace WorldSkillsAPI.Interfaces;

public interface IReminderService
{
    Task<IActionResult> SendCourseReminderAsync(
        int courseId,
        CourseReminderRequest request,
        string token,
        CancellationToken cancellationToken = default);

    Task<IActionResult> SendStudentReminderAsync(
        int courseId,
        int studentId,
        CourseReminderRequest request,
        string token,
        CancellationToken cancellationToken = default);
}
