using Microsoft.AspNetCore.SignalR;
using WorldSkillsAPI.DatabaseContext;
using WorldSkillsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace WorldSkillsAPI.Hubs
{
    public class LessonChatHub : Hub
    {
        private readonly ContextDb _context;

        public LessonChatHub(ContextDb context)
        {
            _context = context;
        }

        public async Task JoinLesson(int lessonId)
        {
            var lessonExists = await _context.Lessons.AnyAsync(l => l.Id == lessonId);
            if (!lessonExists)
            {
                await Clients.Caller.SendAsync("Error", "Урок не найден");
                return;
            }

            string groupName = $"lesson_{lessonId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("UserJoined", Context.ConnectionId);
        }

        public async Task LeaveLesson(int lessonId)
        {
            string groupName = $"lesson_{lessonId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("UserLeft", Context.ConnectionId);
        }

        public async Task SendComment(int lessonId, string content, string token)
        {
            var session = await _context.Sessions
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Token == token);

            if (session == null)
            {
                await Clients.Caller.SendAsync("Error", "Сессия не найдена");
                return;
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                await Clients.Caller.SendAsync("Error", "Комментарий не может быть пустым");
                return;
            }

            var message = new Message
            {
                Content = content,
                CreatedAt = DateTime.UtcNow,
                UserId = session.UserId,
                LessonId = lessonId
            };

            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();

            string groupName = $"lesson_{lessonId}";

            await Clients.Group(groupName).SendAsync("ReceiveComment", new
            {
                id = message.Id,
                content = message.Content,
                createdAt = message.CreatedAt,
                userId = message.UserId,
                userName = session.User?.Name,
                lessonId = message.LessonId
            });
        }
    }
}