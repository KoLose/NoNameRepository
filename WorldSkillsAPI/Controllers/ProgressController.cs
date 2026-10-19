using WorldSkillsAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WorldSkillsAPI.Controllers
{
    [ApiController]
    public class ProgressController : Controller
    {
        private readonly IProgressService _progressService;

        public ProgressController(IProgressService progressService)
        {
            _progressService = progressService;
        }

        [HttpGet]
        [Route("MyProgress")]
        [RoleAuthorized(1)]
        public async Task<IActionResult> MyProgress()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault();
            return await _progressService.GetUserProgressAsync(token);
        }

        [HttpGet]
        [Route("CourseProgress/{courseId}")]
        [RoleAuthorized(1)]
        public async Task<IActionResult> CourseProgress(int courseId)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault();
            return await _progressService.GetCourseProgressAsync(token, courseId);
        }

        [HttpPost]
        [Route("StartLesson/{lessonId}")]
        [RoleAuthorized(1)]
        public async Task<IActionResult> StartLesson(int lessonId)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault();
            return await _progressService.StartLessonAsync(token, lessonId);
        }

        [HttpPost]
        [Route("CompleteLesson/{lessonId}")]
        [RoleAuthorized(1)]
        public async Task<IActionResult> CompleteLesson(int lessonId)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault();
            return await _progressService.CompleteLessonAsync(token, lessonId);
        }
    }
}
