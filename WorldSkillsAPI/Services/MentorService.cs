using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldSkillsAPI.DatabaseContext;
using WorldSkillsAPI.Interfaces;

namespace WorldSkillsAPI.Services;

public class MentorService : IMentorService
{
    private const int RoleMentor = 2;
    private const int RoleStudent = 1;
    private const int StatusInProgress = 2;
    private const int StatusCompleted = 3;

    private readonly ContextDb _context;

    public MentorService(ContextDb context)
    {
        _context = context;
    }

    public async Task<IActionResult> GetOwnCoursesAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        var mentor = await GetMentorAsync(token, cancellationToken);
        if (mentor == null)
            return SessionNotFound();

        if (mentor.Value.RoleId != RoleMentor)
            return AccessDenied();

        var courses = await _context.Courses
            .Where(course => course.MentorId == mentor.Value.UserId)
            .OrderBy(course => course.Id)
            .Select(course => new
            {
                id = course.Id,
                name = course.Name,
                description = course.Description,
                totalLessons = _context.Lessons.Count(lesson => lesson.CourseId == course.Id)
            })
            .ToListAsync(cancellationToken);

        var courseIds = courses.Select(course => course.id).ToList();
        var progress = await _context.LessonProgresses
            .Where(item =>
                courseIds.Contains(item.Lesson.CourseId) &&
                item.User.RoleId == RoleStudent &&
                (item.StatusId == StatusInProgress || item.StatusId == StatusCompleted))
            .Select(item => new
            {
                courseId = item.Lesson.CourseId,
                studentId = item.UserId,
                studentName = item.User.Name,
                email = item.User.Login,
                item.LessonId,
                item.StatusId
            })
            .ToListAsync(cancellationToken);

        var result = courses.Select(course => new
        {
            course.id,
            course.name,
            course.description,
            course.totalLessons,
            students = progress
                .Where(item => item.courseId == course.id)
                .GroupBy(item => new { item.studentId, item.studentName, item.email })
                .Select(group =>
                {
                    var startedLessons = group.Select(item => item.LessonId).Distinct().Count();
                    var completedLessons = group
                        .Where(item => item.StatusId == StatusCompleted)
                        .Select(item => item.LessonId)
                        .Distinct()
                        .Count();
                    var isCompleted = course.totalLessons > 0 && completedLessons >= course.totalLessons;

                    return new
                    {
                        group.Key.studentId,
                        name = group.Key.studentName,
                        group.Key.email,
                        startedLessons,
                        completedLessons,
                        percent = course.totalLessons == 0
                            ? 0
                            : Math.Min(100, completedLessons * 100 / course.totalLessons),
                        isCompleted,
                        statusName = isCompleted ? "Завершён" : "В процессе"
                    };
                })
                .OrderBy(student => student.name)
                .ToList()
        }).ToList();

        return new OkObjectResult(new
        {
            status = true,
            courses = result
        });
    }

    public async Task<IActionResult> GetOtherCoursesAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        var mentor = await GetMentorAsync(token, cancellationToken);
        if (mentor == null)
            return SessionNotFound();

        if (mentor.Value.RoleId != RoleMentor)
            return AccessDenied();

        var courses = await _context.Courses
            .Where(course => course.MentorId != mentor.Value.UserId)
            .OrderBy(course => course.Id)
            .Select(course => new
            {
                id = course.Id,
                name = course.Name,
                description = course.Description,
                mentorId = course.MentorId,
                mentorName = course.Mentor.Name
            })
            .ToListAsync(cancellationToken);

        return new OkObjectResult(new
        {
            status = true,
            courses
        });
    }

    private async Task<(int UserId, int RoleId)?> GetMentorAsync(
        string token,
        CancellationToken cancellationToken)
    {
        var session = await _context.Sessions
            .Where(session => session.Token == token)
            .Select(session => new
            {
                session.UserId,
                session.User.RoleId
            })
            .FirstOrDefaultAsync(cancellationToken);

        return session == null ? null : (session.UserId, session.RoleId);
    }

    private static UnauthorizedObjectResult SessionNotFound() => new(new
    {
        status = false,
        message = "Сессия не найдена"
    });

    private static ObjectResult AccessDenied() => new(new
    {
        status = false,
        message = "Раздел доступен только наставникам"
    })
    { StatusCode = StatusCodes.Status403Forbidden };
}
