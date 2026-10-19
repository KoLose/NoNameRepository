using WorldSkillsAPI.DatabaseContext;
using WorldSkillsAPI.Interfaces;
using WorldSkillsAPI.Models;
using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WorldSkillsAPI.Services
{
    public class GradeService : IGradeService
    {
        private readonly ContextDb _context;

        private const int AnswerStatusAccepted = 3;

        private const int RoleUser = 1;
        private const int RoleMentor = 2;
        private const int RoleAdmin = 3;

        public GradeService(ContextDb context)
        {
            _context = context;
        }

        private async Task<Session?> GetSessionByTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            return await _context.Sessions
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Token == token);
        }

        public async Task<IActionResult> GetGradesByAnswerAsync(int answerId, string token)
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

                var answer = await _context.Answers
                    .Include(item => item.Assignment)
                    .ThenInclude(assignment => assignment.Lesson)
                    .ThenInclude(lesson => lesson.Course)
                    .FirstOrDefaultAsync(a => a.Id == answerId);
                if (answer == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Ответ не найден"
                    });
                }

                if (session.User.RoleId == RoleMentor &&
                    answer.Assignment?.Lesson?.Course?.MentorId != session.UserId)
                {
                    return new ObjectResult(new
                    {
                        status = false,
                        message = "Можно просматривать оценки только в своих курсах"
                    })
                    { StatusCode = 403 };
                }

                var grades = await _context.Grades
                    .Include(g => g.Mentor)
                    .Where(g => g.AnswerId == answerId)
                    .Select(g => new
                    {
                        id = g.Id,
                        score = g.Score,
                        gradedAt = g.GradedAt,
                        mentorId = g.MentorId,
                        mentorName = g.Mentor != null ? g.Mentor.Name : null
                    })
                    .ToListAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    grades
                });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при получении оценок: {innerMessage}"
                });
            }
        }

        public async Task<IActionResult> GetMyGradesAsync(string token)
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

                int userId = session.UserId;

                var grades = await _context.Grades
                    .Include(g => g.Answer)
                    .ThenInclude(a => a.Assignment)
                    .Include(g => g.Mentor)
                    .Where(g => g.Answer != null && g.Answer.UserId == userId)
                    .Select(g => new
                    {
                        id = g.Id,
                        answerId = g.AnswerId,
                        score = g.Score,
                        gradedAt = g.GradedAt,
                        mentorId = g.MentorId,
                        mentorName = g.Mentor != null ? g.Mentor.Name : null,
                        assignmentId = g.Answer != null ? g.Answer.AssignmentId : 0,
                        assignmentName = g.Answer != null && g.Answer.Assignment != null
                            ? g.Answer.Assignment.Name : null
                    })
                    .ToListAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    grades
                });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при получении моих оценок: {innerMessage}"
                });
            }
        }
        public async Task<IActionResult> GetGradesByMentorAsync(string token)
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

                int mentorId = session.UserId;

                var grades = await _context.Grades
                    .Include(g => g.Answer)
                    .ThenInclude(a => a.Assignment)
                    .Where(g => g.MentorId == mentorId)
                    .Select(g => new
                    {
                        id = g.Id,
                        answerId = g.AnswerId,
                        score = g.Score,
                        gradedAt = g.GradedAt,
                        assignmentId = g.Answer != null ? g.Answer.AssignmentId : 0,
                        assignmentName = g.Answer != null && g.Answer.Assignment != null
                            ? g.Answer.Assignment.Name : null
                    })
                    .ToListAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    grades
                });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при получении оценок наставника: {innerMessage}"
                });
            }
        }

        public async Task<IActionResult> CreateGradeAsync(GradeRequest request, string token)
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

                int mentorId = session.UserId;

                var answer = await _context.Answers
                    .Include(a => a.Assignment)
                    .ThenInclude(assignment => assignment.Lesson)
                    .ThenInclude(lesson => lesson.Course)
                    .FirstOrDefaultAsync(a => a.Id == request.AnswerId);

                if (answer == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Ответ не найден"
                    });
                }

                if (session.User.RoleId == RoleMentor &&
                    answer.Assignment?.Lesson?.Course?.MentorId != session.UserId)
                {
                    return new ObjectResult(new
                    {
                        status = false,
                        message = "Можно оценивать ответы только в своих курсах"
                    })
                    { StatusCode = 403 };
                }

                if (request.Score < 0)
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Балл не может быть отрицательным"
                    });
                }

                if (answer.Assignment != null && request.Score > answer.Assignment.MaxScore)
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = $"Балл не может превышать {answer.Assignment.MaxScore}"
                    });
                }

                var existing = await _context.Grades
                    .FirstOrDefaultAsync(g => g.AnswerId == request.AnswerId && g.MentorId == mentorId);

                if (existing != null)
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Вы уже ставили оценку этому ответу. Используйте Update"
                    });
                }

                var grade = new Grade
                {
                    AnswerId = request.AnswerId,
                    Score = request.Score,
                    GradedAt = DateTime.UtcNow,
                    MentorId = mentorId
                };

                await _context.Grades.AddAsync(grade);

                answer.StatusId = AnswerStatusAccepted;

                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    gradeId = grade.Id
                });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при выставлении оценки: {innerMessage}"
                });
            }
        }

        public async Task<IActionResult> UpdateGradeAsync(int id, GradeRequest request, string token)
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

                int userId = session.UserId;
                int roleId = session.User.RoleId;

                var grade = await _context.Grades
                    .Include(g => g.Answer)
                    .ThenInclude(a => a.Assignment)
                    .ThenInclude(assignment => assignment.Lesson)
                    .ThenInclude(lesson => lesson.Course)
                    .FirstOrDefaultAsync(g => g.Id == id);

                if (grade == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Оценка не найдена"
                    });
                }

                if (roleId == RoleMentor &&
                    (grade.MentorId != userId ||
                     grade.Answer?.Assignment?.Lesson?.Course?.MentorId != userId))
                {
                    return new ObjectResult(new
                    {
                        status = false,
                        message = "Можно редактировать оценки только в своих курсах"
                    })
                    { StatusCode = 403 };
                }

                if (roleId != RoleMentor && roleId != RoleAdmin)
                {
                    return new ObjectResult(new
                    {
                        status = false,
                        message = "Нет прав на изменение оценки"
                    })
                    { StatusCode = 403 };
                }

                if (request.Score < 0)
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Балл не может быть отрицательным"
                    });
                }

                if (grade.Answer?.Assignment != null && request.Score > grade.Answer.Assignment.MaxScore)
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = $"Балл не может превышать {grade.Answer.Assignment.MaxScore}"
                    });
                }

                grade.Score = request.Score;
                grade.GradedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    message = "Оценка обновлена"
                });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при обновлении оценки: {innerMessage}"
                });
            }
        }
    }
}
