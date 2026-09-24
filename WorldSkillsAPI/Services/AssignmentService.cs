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

        public AssignmentService(ContextDb context)
        {
            _context = context;
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

        public async Task<IActionResult> CreateAssignmentAsync(AssignmentRequest request)
        {
            try
            {
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

                var lesson = await _context.Lessons.FirstOrDefaultAsync(l => l.Id == request.LessonId);
                if (lesson == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Урок не найден"
                    });
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

        public async Task<IActionResult> UpdateAssignmentAsync(int id, AssignmentRequest request)
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