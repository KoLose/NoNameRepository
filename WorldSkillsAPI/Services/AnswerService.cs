using WorldSkillsAPI.DatabaseContext;
using WorldSkillsAPI.Interfaces;
using WorldSkillsAPI.Models;
using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WorldSkillsAPI.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly ContextDb _context;

        // статусы ответа
        private const int StatusNotGiven = 1;   // не дан
        private const int StatusOnReview = 2;   // на проверке
        private const int StatusAccepted = 3;   // принят
        private const int StatusRejected = 4;   // отклонён

        public AnswerService(ContextDb context)
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

        public async Task<IActionResult> GetAnswersByAssignmentAsync(int assignmentId)
        {
            try
            {
                var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.Id == assignmentId);
                if (assignment == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Задание не найдено"
                    });
                }

                var answers = await _context.Answers
                    .Include(a => a.User)
                    .Include(a => a.Status)
                    .Where(a => a.AssignmentId == assignmentId)
                    .Select(a => new
                    {
                        id = a.Id,
                        userId = a.UserId,
                        userName = a.User != null ? a.User.Name : null,
                        content = a.Content,
                        fileUrl = a.FileUrl,
                        submittedAt = a.SubmittedAt,
                        statusId = a.StatusId,
                        statusName = a.Status != null ? a.Status.Name : null
                    })
                    .ToListAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    answers
                });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при получении ответов: {innerMessage}"
                });
            }
        }

        public async Task<IActionResult> GetAnswersByUserAsync(string token)
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

                var answers = await _context.Answers
                    .Include(a => a.Assignment)
                    .Include(a => a.Status)
                    .Where(a => a.UserId == userId.Value)
                    .Select(a => new
                    {
                        id = a.Id,
                        assignmentId = a.AssignmentId,
                        assignmentName = a.Assignment != null ? a.Assignment.Name : null,
                        content = a.Content,
                        fileUrl = a.FileUrl,
                        submittedAt = a.SubmittedAt,
                        statusId = a.StatusId,
                        statusName = a.Status != null ? a.Status.Name : null
                    })
                    .ToListAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    answers
                });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при получении ответов пользователя: {innerMessage}"
                });
            }
        }

        public async Task<IActionResult> GetAnswerByIdAsync(int id, string token)
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

                var session = await _context.Sessions
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Token == token);

                int roleId = session!.User.RoleId;

                var answer = await _context.Answers
                    .Include(a => a.User)
                    .Include(a => a.Assignment)
                    .Include(a => a.Status)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (answer == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Ответ не найден"
                    });
                }

                bool isMentorOrAdmin = roleId == 2 || roleId == 3;

                if (!isMentorOrAdmin && answer.UserId != userId.Value)
                {
                    return new ObjectResult(new
                    {
                        status = false,
                        message = "Нет доступа к чужому ответу"
                    })
                    { StatusCode = 403 };
                }

                return new OkObjectResult(new
                {
                    status = true,
                    answer = new
                    {
                        id = answer.Id,
                        userId = answer.UserId,
                        userName = answer.User != null ? answer.User.Name : null,
                        assignmentId = answer.AssignmentId,
                        assignmentName = answer.Assignment != null ? answer.Assignment.Name : null,
                        content = answer.Content,
                        fileUrl = answer.FileUrl,
                        submittedAt = answer.SubmittedAt,
                        statusId = answer.StatusId,
                        statusName = answer.Status != null ? answer.Status.Name : null
                    }
                });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при получении ответа: {innerMessage}"
                });
            }
        }

        public async Task<IActionResult> SubmitAnswerAsync(AnswerRequest request, string token)
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

                var assignment = await _context.Assignments
                    .FirstOrDefaultAsync(a => a.Id == request.AssignmentId);

                if (assignment == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Задание не найдено"
                    });
                }

                var existing = await _context.Answers
                    .FirstOrDefaultAsync(a => a.AssignmentId == request.AssignmentId && a.UserId == userId.Value);

                if (existing != null)
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Вы уже отправляли ответ на это задание"
                    });
                }

                if (string.IsNullOrWhiteSpace(request.Content) && string.IsNullOrWhiteSpace(request.FileUrl))
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Нужно приложить текст или файл"
                    });
                }

                var answer = new Answer
                {
                    AssignmentId = request.AssignmentId,
                    UserId = userId.Value,
                    Content = request.Content,
                    FileUrl = request.FileUrl,
                    SubmittedAt = DateTime.UtcNow,
                    StatusId = StatusOnReview
                };

                await _context.Answers.AddAsync(answer);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    answerId = answer.Id
                });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при отправке ответа: {innerMessage}"
                });
            }
        }

        public async Task<IActionResult> UpdateAnswerAsync(int id, AnswerRequest request, string token)
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

                var answer = await _context.Answers.FirstOrDefaultAsync(a => a.Id == id);
                if (answer == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Ответ не найден"
                    });
                }

                if (answer.UserId != userId.Value)
                {
                    return new ObjectResult(new
                    {
                        status = false,
                        message = "Можно редактировать только свой ответ"
                    })
                    { StatusCode = 403 };
                }

                if (answer.StatusId == StatusAccepted || answer.StatusId == StatusRejected)
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Ответ уже проверен, изменения невозможны"
                    });
                }

                if (request.Content != null)
                    answer.Content = request.Content;

                if (request.FileUrl != null)
                    answer.FileUrl = request.FileUrl;

                answer.StatusId = StatusOnReview;
                answer.SubmittedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    message = "Ответ обновлён"
                });
            }
            catch (Exception ex)
            {
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при обновлении ответа: {innerMessage}"
                });
            }
        }
    }
}