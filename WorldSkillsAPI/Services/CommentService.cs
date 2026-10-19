using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldSkillsAPI.DatabaseContext;
using WorldSkillsAPI.Interfaces;
using WorldSkillsAPI.Models;
using WorldSkillsAPI.Requests;

namespace WorldSkillsAPI.Services
{
    public class CommentService : ICommentService
    {
        private const int MaxCommentLength = 2000;
        private readonly ContextDb _context;

        public CommentService(ContextDb context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetCommentsAsync(int lessonId)
        {
            try
            {
                var lessonExists = await _context.Lessons.AnyAsync(lesson => lesson.Id == lessonId);
                if (!lessonExists)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Урок не найден"
                    });
                }

                var comments = await _context.Messages
                    .Include(message => message.User)
                    .Where(message => message.LessonId == lessonId)
                    .OrderBy(message => message.CreatedAt)
                    .Select(message => new
                    {
                        id = message.Id,
                        content = message.Content,
                        createdAt = message.CreatedAt,
                        userId = message.UserId,
                        userName = message.User != null ? message.User.Name : "Пользователь",
                        lessonId = message.LessonId
                    })
                    .ToListAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    comments
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при получении комментариев: {ex.Message}"
                });
            }
        }

        public async Task<IActionResult> AddCommentAsync(string token, int lessonId, CommentRequest request)
        {
            try
            {
                var session = await _context.Sessions
                    .Include(item => item.User)
                    .FirstOrDefaultAsync(item => item.Token == token);

                if (session == null)
                {
                    return new UnauthorizedObjectResult(new
                    {
                        status = false,
                        message = "Сессия не найдена"
                    });
                }

                var lessonExists = await _context.Lessons.AnyAsync(lesson => lesson.Id == lessonId);
                if (!lessonExists)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Урок не найден"
                    });
                }

                var content = request.Content?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(content))
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Комментарий не может быть пустым"
                    });
                }

                if (content.Length > MaxCommentLength)
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = $"Комментарий не должен превышать {MaxCommentLength} символов"
                    });
                }

                var comment = new Message
                {
                    Content = content,
                    CreatedAt = DateTime.UtcNow,
                    UserId = session.UserId,
                    LessonId = lessonId
                };

                await _context.Messages.AddAsync(comment);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    message = "Комментарий отправлен",
                    comment = new
                    {
                        id = comment.Id,
                        content = comment.Content,
                        createdAt = comment.CreatedAt,
                        userId = comment.UserId,
                        userName = session.User?.Name ?? "Пользователь",
                        lessonId = comment.LessonId
                    }
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при отправке комментария: {ex.Message}"
                });
            }
        }
    }
}
