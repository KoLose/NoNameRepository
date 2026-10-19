using WorldSkillsAPI.DatabaseContext;
using WorldSkillsAPI.Interfaces;
using WorldSkillsAPI.Models;
using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WorldSkillsAPI.Services
{
    public class LessonService : ILessonService
    {
        private readonly ContextDb _context;

        public LessonService(ContextDb context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetLessonsByCourseAsync(int courseId)
        {
            try
            {
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
                if (course == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Курс не найден"
                    });
                }

                var lessons = await _context.Lessons
                    .Where(l => l.CourseId == courseId)
                    .OrderBy(l => l.Position)
                    .Select(l => new
                    {
                        id = l.Id,
                        name = l.Name,
                        content = l.Content,
                        videoUrl = l.VideoUrl,
                        position = l.Position,
                        courseId = l.CourseId
                    })
                    .ToListAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    lessons
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при получении уроков: {ex.Message}"
                });
            }
        }

        public async Task<IActionResult> GetLessonByIdAsync(int id)
        {
            try
            {
                var lesson = await _context.Lessons
                    .Include(l => l.Course)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (lesson == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Урок не найден"
                    });
                }

                return new OkObjectResult(new
                {
                    status = true,
                    lesson = new
                    {
                        id = lesson.Id,
                        name = lesson.Name,
                        content = lesson.Content,
                        videoUrl = lesson.VideoUrl,
                        position = lesson.Position,
                        courseId = lesson.CourseId,
                        courseName = lesson.Course != null ? lesson.Course.Name : null,
                        courseMentorId = lesson.Course?.MentorId ?? 0
                    }
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при получении урока: {ex.Message}"
                });
            }
        }

        public async Task<IActionResult> CreateLessonAsync(LessonRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Название урока обязательно"
                    });
                }

                var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == request.CourseId);
                if (course == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Курс не найден"
                    });
                }

                if (request.Position <= 0)
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Порядок урока должен быть больше нуля"
                    });
                }

                var lesson = new Lesson
                {
                    Name = request.Name,
                    Content = request.Content,
                    VideoUrl = request.VideoUrl,
                    Position = request.Position,
                    CourseId = request.CourseId
                };

                await _context.Lessons.AddAsync(lesson);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    lessonId = lesson.Id
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при создании урока: {ex.Message}"
                });
            }
        }

        public async Task<IActionResult> UpdateLessonAsync(int id, LessonRequest request)
        {
            try
            {
                var lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == id);
                if (lesson == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Урок не найден"
                    });
                }

                if (!string.IsNullOrWhiteSpace(request.Name))
                    lesson.Name = request.Name;

                if (request.Content != null)
                    lesson.Content = request.Content;

                if (request.VideoUrl != null)
                    lesson.VideoUrl = request.VideoUrl;

                if (request.Position > 0)
                    lesson.Position = request.Position;

                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    message = "Урок обновлён"
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при обновлении урока: {ex.Message}"
                });
            }
        }

        public async Task<IActionResult> DeleteLessonAsync(int id)
        {
            try
            {
                var lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == id);
                if (lesson == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Урок не найден"
                    });
                }

                _context.Lessons.Remove(lesson);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    message = "Урок удалён"
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при удалении урока: {ex.Message}"
                });
            }
        }
    }
}
