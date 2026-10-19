using WorldSkillsAPI.DatabaseContext;
using WorldSkillsAPI.Interfaces;
using WorldSkillsAPI.Models;
using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WorldSkillsAPI.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly ContextDb _context;

        private const int RoleMentor = 2;
        private const int RoleAdmin = 3;

        public AssignmentService(ContextDb context)
        {
            _context = context;
        }

        private async Task<Session?> GetSessionByTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            return await _context.Sessions
                .Include(session => session.User)
                .FirstOrDefaultAsync(session => session.Token == token);
        }

        public async Task<IActionResult> GetAssignmentsByLessonAsync(int lessonId)
        {
            try
            {
                var lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == lessonId);
                if (lesson == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Урок не найден"
                    });
                }

                var assignments = await _context.Assignments
                    .Where(a => a.LessonId == lessonId)
                    .OrderBy(a => a.Position)
                    .Select(a => new
                    {
                        id = a.Id,
                        name = a.Name,
                        description = a.Description,
                        maxScore = a.MaxScore,
                        position = a.Position,
                        lessonId = a.LessonId
                    })
                    .ToListAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    assignments
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при получении заданий: {ex.Message}"
                });
            }
        }

        public async Task<IActionResult> GetAssignmentByIdAsync(int id)
        {
            try
            {
                var assignment = await _context.Assignments
                    .Include(a => a.Lesson)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (assignment == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Задание не найдено"
                    });
                }

                return new OkObjectResult(new
                {
                    status = true,
                    assignment = new
                    {
                        id = assignment.Id,
                        name = assignment.Name,
                        description = assignment.Description,
                        maxScore = assignment.MaxScore,
                        position = assignment.Position,
                        lessonId = assignment.LessonId,
                        lessonName = assignment.Lesson != null ? assignment.Lesson.Name : null
                    }
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при получении задания: {ex.Message}"
                });
            }
        }

        public async Task<IActionResult> CreateAssignmentAsync(AssignmentRequest request, string token)
        {
            try
            {
                var session = await GetSessionByTokenAsync(token);
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
                        message = "Нет прав на создание заданий"
                    })
                    { StatusCode = 403 };
                }

                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Название задания обязательно"
                    });
                }

                if (request.MaxScore <= 0)
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Максимальный балл должен быть больше нуля"
                    });
                }

                if (request.Position <= 0)
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Порядок задания должен быть больше нуля"
                    });
                }

                var lesson = await _context.Lessons
                    .Include(item => item.Course)
                    .FirstOrDefaultAsync(item => item.Id == request.LessonId);
                if (lesson == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Урок не найден"
                    });
                }

                if (session.User.RoleId == RoleMentor && lesson.Course?.MentorId != session.UserId)
                {
                    return new ObjectResult(new
                    {
                        status = false,
                        message = "Можно создавать задания только в своих курсах"
                    })
                    { StatusCode = 403 };
                }

                var assignment = new Assignment
                {
                    LessonId = request.LessonId,
                    Name = request.Name,
                    Description = request.Description,
                    MaxScore = request.MaxScore,
                    Position = request.Position
                };

                await _context.Assignments.AddAsync(assignment);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    assignmentId = assignment.Id
                });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при создании задания: {innerMessage}"
                });
            }
        }

        public async Task<IActionResult> UpdateAssignmentAsync(int id, AssignmentRequest request, string token)
        {
            try
            {
                var session = await GetSessionByTokenAsync(token);
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
                        message = "Нет прав на изменение заданий"
                    })
                    { StatusCode = 403 };
                }

                var assignment = await _context.Assignments
                    .Include(item => item.Lesson)
                    .ThenInclude(lesson => lesson.Course)
                    .FirstOrDefaultAsync(item => item.Id == id);
                if (assignment == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Задание не найдено"
                    });
                }

                if (session.User.RoleId == RoleMentor &&
                    assignment.Lesson?.Course?.MentorId != session.UserId)
                {
                    return new ObjectResult(new
                    {
                        status = false,
                        message = "Можно изменять задания только в своих курсах"
                    })
                    { StatusCode = 403 };
                }

                if (!string.IsNullOrWhiteSpace(request.Name))
                    assignment.Name = request.Name;

                if (request.Description != null)
                    assignment.Description = request.Description;

                if (request.MaxScore > 0)
                    assignment.MaxScore = request.MaxScore;

                if (request.Position > 0)
                    assignment.Position = request.Position;


                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    message = "Задание обновлено"
                });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при обновлении задания: {innerMessage}"
                });
            }
        }

        public async Task<IActionResult> DeleteAssignmentAsync(int id)
        {
            try
            {
                var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.Id == id);
                if (assignment == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Задание не найдено"
                    });
                }

                _context.Assignments.Remove(assignment);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    message = "Задание удалено"
                });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при удалении задания: {innerMessage}"
                });
            }
        }
    }
}
