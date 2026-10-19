using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldSkillsAPI.DatabaseContext;
using WorldSkillsAPI.Interfaces;
using WorldSkillsAPI.Models;
using WorldSkillsAPI.Requests;

namespace WorldSkillsAPI.Services;

public class ReminderService : IReminderService
{
    private const int RoleStudent = 1;
    private const int RoleMentor = 2;
    private const int RoleAdmin = 3;
    private const int StatusInProgress = 2;
    private const int StatusCompleted = 3;
    private const int OneTimePeriodicity = 1;
    private const int MaxTextLength = 2000;

    private readonly ContextDb _context;
    private readonly IEmailService _emailService;
    private readonly ILogger<ReminderService> _logger;

    public ReminderService(
        ContextDb context,
        IEmailService emailService,
        ILogger<ReminderService> logger)
    {
        _context = context;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<IActionResult> SendCourseReminderAsync(
        int courseId,
        CourseReminderRequest request,
        string token,
        CancellationToken cancellationToken = default)
    {
        var session = await _context.Sessions
            .Include(item => item.User)
            .FirstOrDefaultAsync(item => item.Token == token, cancellationToken);

        if (session == null)
        {
            return new UnauthorizedObjectResult(new
            {
                status = false,
                message = "Сессия не найдена"
            });
        }

        if (session.User.RoleId != RoleMentor && session.User.RoleId != RoleAdmin)
        {
            return new ObjectResult(new
            {
                status = false,
                message = "Нет прав на отправку напоминаний"
            })
            { StatusCode = 403 };
        }

        var text = (request.Text ?? "").Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            return new BadRequestObjectResult(new
            {
                status = false,
                message = "Введите текст напоминания"
            });
        }

        if (text.Length > MaxTextLength)
        {
            return new BadRequestObjectResult(new
            {
                status = false,
                message = $"Напоминание не должно превышать {MaxTextLength} символов"
            });
        }

        var course = await _context.Courses
            .FirstOrDefaultAsync(item => item.Id == courseId, cancellationToken);
        if (course == null)
        {
            return new NotFoundObjectResult(new
            {
                status = false,
                message = "Курс не найден"
            });
        }

        if (session.User.RoleId == RoleMentor && course.MentorId != session.UserId)
        {
            return new ObjectResult(new
            {
                status = false,
                message = "Можно отправлять напоминания только участникам своих курсов"
            })
            { StatusCode = 403 };
        }

        if (!_emailService.IsConfigured)
        {
            return new BadRequestObjectResult(new
            {
                status = false,
                message = "SMTP не настроен. Добавьте настройки Email в user-secrets или переменные окружения"
            });
        }

        var totalLessons = await _context.Lessons
            .CountAsync(item => item.CourseId == courseId, cancellationToken);
        if (totalLessons == 0)
        {
            return new BadRequestObjectResult(new
            {
                status = false,
                message = "В курсе нет уроков"
            });
        }

        var courseProgress = await _context.LessonProgresses
            .Where(item =>
                item.Lesson.CourseId == courseId &&
                item.User.RoleId == RoleStudent &&
                (item.StatusId == StatusInProgress || item.StatusId == StatusCompleted))
            .Select(item => new
            {
                item.UserId,
                item.User.Name,
                Email = item.User.Login,
                item.LessonId,
                item.StatusId
            })
            .ToListAsync(cancellationToken);

        var recipients = courseProgress
            .GroupBy(item => new { item.UserId, item.Name, item.Email })
            .Where(group => group
                .Where(item => item.StatusId == StatusCompleted)
                .Select(item => item.LessonId)
                .Distinct()
                .Count() < totalLessons)
            .Select(group => new
            {
                group.Key.UserId,
                group.Key.Name,
                group.Key.Email
            })
            .ToList();

        if (recipients.Count == 0)
        {
            return new BadRequestObjectResult(new
            {
                status = false,
                message = "Нет студентов, которые начали курс и ещё не завершили его"
            });
        }

        var sentUserIds = new List<int>();
        var failedCount = 0;
        var subject = $"Напоминание по курсу «{course.Name}»";
        var body = $"Здравствуйте!\n\n{text}\n\nКурс: {course.Name}";

        foreach (var recipient in recipients)
        {
            try
            {
                await _emailService.SendAsync(
                    recipient.Name,
                    recipient.Email,
                    subject,
                    body,
                    cancellationToken);
                sentUserIds.Add(recipient.UserId);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                failedCount++;
                _logger.LogWarning(
                    ex,
                    "Не удалось отправить напоминание пользователю {UserId}",
                    recipient.UserId);
            }
        }

        if (sentUserIds.Count > 0)
        {
            var sentAt = DateTime.UtcNow;
            var reminders = sentUserIds.Select(userId => new Reminder
            {
                UserId = userId,
                CourseId = courseId,
                Text = text,
                RemindAt = sentAt,
                PeriodicityId = OneTimePeriodicity,
                IsRead = false,
                NextTriggerAt = null
            });

            await _context.Reminders.AddRangeAsync(reminders, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        if (sentUserIds.Count == 0)
        {
            return new BadRequestObjectResult(new
            {
                status = false,
                message = "Не удалось отправить ни одного письма",
                sentCount = 0,
                failedCount
            });
        }

        return new OkObjectResult(new
        {
            status = true,
            message = failedCount == 0
                ? $"Напоминание отправлено: {sentUserIds.Count}"
                : $"Отправлено: {sentUserIds.Count}, ошибок: {failedCount}",
            sentCount = sentUserIds.Count,
            failedCount
        });
    }
}
