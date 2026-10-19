using Microsoft.AspNetCore.Mvc;
using WorldSkillsAPI.Requests;

namespace WorldSkillsAPI.Interfaces
{
    public interface ICommentService
    {
        Task<IActionResult> GetCommentsAsync(int lessonId);
        Task<IActionResult> AddCommentAsync(string token, int lessonId, CommentRequest request);
    }
}
