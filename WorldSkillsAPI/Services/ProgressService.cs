using WorldSkillsAPI.DatabaseContext;
using WorldSkillsAPI.Interfaces;
using WorldSkillsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WorldSkillsAPI.Services
{
    public class ProgressService : IProgressService
    {
        private readonly ContextDb _context;

        // статусы прогресса
        private const int StatusNotStarted = 1; // не начат
        private const int StatusInProgress = 2; // в процессе
        private const int StatusCompleted = 3;  // завершён

        public ProgressService(ContextDb context)
        {
            _context = context;
        }

        private async Task<int?> GetUserIdByTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var session = await _context.Sessions
                .FirstOrDefaultAsync(s => s.Token == token);

            return session?.UserId;
        }

        public async Task<IActionResult> GetUserProgressAsync(string token)
        {
            try
            {
                var userId = await GetUserIdByTokenAsync(token);
                if (userId == null)
                {
                    return new UnauthorizedObjectResult(new
                    {
                        status = false,
                        message = "Сессия не найдена"
                    });
                }

                var progress = await _context.LessonProgresses
                    .Include(p => p.Lesson)
                    .Include(p => p.Status)
                    .Where(p => p.UserId == userId.Value)
                    .Select(p => new
                    {
                        lessonId = p.LessonId,
                        lessonName = p.Lesson != null ? p.Lesson.Name : null,
                        courseId = p.Lesson != null ? p.Lesson.CourseId : 0,
                        statusId = p.StatusId,
                        statusName = p.Status != null ? p.Status.Name : null,
                        startedAt = p.StartedAt,
                        completedAt = p.CompletedAt
                    })
                    .ToListAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    progress
                });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при получении прогресса: {innerMessage}"
                });
            }
        }

        public async Task<IActionResult> GetCourseProgressAsync(string token, int courseId)
        {
            try
            {
                var userId = await GetUserIdByTokenAsync(token);
                if (userId == null)
                {
                    return new UnauthorizedObjectResult(new
                    {
                        status = false,
                        message = "Сессия не найдена"
                    });
                }

                var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
                if (course == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Курс не найден"
                    });
                }

                var progress = await _context.LessonProgresses
                    .Include(p => p.Lesson)
                    .Include(p => p.Status)
                    .Where(p => p.UserId == userId.Value && p.Lesson != null && p.Lesson.CourseId == courseId)
                    .Select(p => new
                    {
                        lessonId = p.LessonId,
                        lessonName = p.Lesson != null ? p.Lesson.Name : null,
                        statusId = p.StatusId,
                        statusName = p.Status != null ? p.Status.Name : null,
                        startedAt = p.StartedAt,
                        completedAt = p.CompletedAt
                    })
                    .ToListAsync();

                var totalLessons = await _context.Lessons.CountAsync(l => l.CourseId == courseId);
                var completedLessons = progress.Count(p => p.statusId == StatusCompleted);
                int percent = totalLessons == 0 ? 0 : (completedLessons * 100) / totalLessons;

                return new OkObjectResult(new
                {
                    status = true,
                    courseId,
                    courseName = course.Name,
                    totalLessons,
                    completedLessons,
                    percent,
                    progress
                });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при получении прогресса по курсу: {innerMessage}"
                });
            }
        }

        public async Task<IActionResult> StartLessonAsync(string token, int lessonId)
        {
            try
            {
                var userId = await GetUserIdByTokenAsync(token);
                if (userId == null)
                {
                    return new UnauthorizedObjectResult(new
                    {
                        status = false,
                        message = "Сессия не найдена"
                    });
                }

                var lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == lessonId);
                if (lesson == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Урок не найден"
                    });
                }

                var existing = await _context.LessonProgresses
                    .FirstOrDefaultAsync(p => p.UserId == userId.Value && p.LessonId == lessonId);

                if (existing != null)
                {
                    if (existing.StatusId == StatusCompleted)
                    {
                        return new BadRequestObjectResult(new
                        {
                            status = false,
                            message = "Урок уже завершён"
                        });
                    }

                    if (existing.StatusId == StatusInProgress)
                    {
                        return new BadRequestObjectResult(new
                        {
                            status = false,
                            message = "Урок уже начат"
                        });
                    }

                    existing.StatusId = StatusInProgress;
                    existing.StartedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    return new OkObjectResult(new
                    {
                        status = true,
                        message = "Урок продолжен"
                    });
                }

                var progress = new LessonProgress
                {
                    UserId = userId.Value,
                    LessonId = lessonId,
                    StatusId = StatusInProgress,
                    StartedAt = DateTime.UtcNow,
                    CompletedAt = null
                };

                await _context.LessonProgresses.AddAsync(progress);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    message = "Урок начат"
                });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при начале урока: {innerMessage}"
                });
            }
        }

        public async Task<IActionResult> CompleteLessonAsync(string token, int lessonId)
        {
            try
            {
                var userId = await GetUserIdByTokenAsync(token);
                if (userId == null)
                {
                    return new UnauthorizedObjectResult(new
                    {
                        status = false,
                        message = "Сессия не найдена"
                    });
                }

                var progress = await _context.LessonProgresses
                    .FirstOrDefaultAsync(p => p.UserId == userId.Value && p.LessonId == lessonId);

                if (progress == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Урок не был начат. Сначала вызовите StartLesson"
                    });
                }

                if (progress.StatusId == StatusCompleted)
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Урок уже завершён"
                    });
                }

                progress.StatusId = StatusCompleted;
                progress.CompletedAt = DateTime.UtcNow;

                if (progress.StartedAt == null)
                    progress.StartedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    message = "Урок завершён"
                });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при завершении урока: {innerMessage}"
                });
            }
        }
    }
}
