using WorldSkillsAPI.Interfaces;
using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace WorldSkillsAPI.Controllers
{
    [ApiController]
    public class LessonController : Controller
    {
        private readonly ILessonService _lessonService;

        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        [HttpGet]
        [Route("GetByCourse/{courseId}")]
        public async Task<IActionResult> GetByCourse(int courseId)
        {
            return await _lessonService.GetLessonsByCourseAsync(courseId);
        }

        [HttpGet]
        [Route("GetLesson/{id}")]
        public async Task<IActionResult> GetLesson(int id)
        {
            return await _lessonService.GetLessonByIdAsync(id);
        }

        [HttpPost]
        [Route("CreateLesson")]
        [RoleAuthorized(2, 3)]
        public async Task<IActionResult> CreateLesson([FromBody] LessonRequest request)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault() ?? "";
            return await _lessonService.CreateLessonAsync(request, token);
        }

        [HttpPut]
        [Route("UpdateLesson/{id}")]
        [RoleAuthorized(2, 3)]
        public async Task<IActionResult> UpdateLesson(int id, [FromBody] LessonRequest request)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault() ?? "";
            return await _lessonService.UpdateLessonAsync(id, request, token);
        }

        [HttpDelete]
        [Route("DeleteLesson/{id}")]
        [RoleAuthorized(3)]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            return await _lessonService.DeleteLessonAsync(id);
        }
    }
}
