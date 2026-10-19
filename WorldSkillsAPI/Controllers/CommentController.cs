using Microsoft.AspNetCore.Mvc;
using WorldSkillsAPI.Interfaces;
using WorldSkillsAPI.Requests;

namespace WorldSkillsAPI.Controllers
{
    [ApiController]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet]
        [Route("Comments/{lessonId}")]
        [RoleAuthorized]
        public async Task<IActionResult> Comments(int lessonId)
        {
            return await _commentService.GetCommentsAsync(lessonId);
        }

        [HttpPost]
        [Route("AddComment/{lessonId}")]
        [RoleAuthorized]
        public async Task<IActionResult> AddComment(int lessonId, [FromBody] CommentRequest request)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault() ?? "";
            return await _commentService.AddCommentAsync(token, lessonId, request);
        }
    }
}
