using WorldSkillsAPI.Interfaces;
using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace WorldSkillsAPI.Controllers
{
    [ApiController]
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        [Route("GetCourses")]
        public async Task<IActionResult> GetCourses()
        {
            return await _courseService.GetCoursesAsync();
        }

        [HttpGet]
        [Route("GetCourse/{id}")]
        public async Task<IActionResult> GetCourse(int id)
        {
            return await _courseService.GetCourseByIdAsync(id);
        }

        [HttpPost]
        [Route("CreateCourse")]
        [RoleAuthorized(2, 3)]
        public async Task<IActionResult> CreateCourse([FromBody] CourseRequest request)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault() ?? "";
            return await _courseService.CreateCourseAsync(request, token);
        }

        [HttpPut]
        [Route("UpdateCourse/{id}")]
        [RoleAuthorized(2, 3)]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseRequest request)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault() ?? "";
            return await _courseService.UpdateCourseAsync(id, request, token);
        }

        [HttpDelete]
        [Route("DeleteCourse/{id}")]
        [RoleAuthorized(3)]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            return await _courseService.DeleteCourseAsync(id);
        }


        [HttpGet]
        [Route("Favorites")]
        [RoleAuthorized(1)]
        public async Task<IActionResult> Favorites()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault();
            return await _courseService.GetFavoriteCoursesAsync(token);
        }

        [HttpPost]
        [Route("AddFavorite/{courseId}")]
        [RoleAuthorized(1)]
        public async Task<IActionResult> AddFavorite(int courseId)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault();
            return await _courseService.AddFavoriteCourseAsync(token, courseId);
        }

        [HttpDelete]
        [Route("RemoveFavorite/{courseId}")]
        [RoleAuthorized(1)]
        public async Task<IActionResult> RemoveFavorite(int courseId)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault();
            return await _courseService.RemoveFavoriteCourseAsync(token, courseId);
        }
    }
}
