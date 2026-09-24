using WorldSkillsAPI.Interfaces;
using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace WorldSkillsAPI.Controllers
{
    [ApiController]
    public class AssignmentController : Controller
    {
        private readonly IAssignmentService _assignmentService;

        public AssignmentController(IAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [HttpGet]
        [Route("GetByLesson/{lessonId}")]
        public async Task<IActionResult> GetByLesson(int lessonId)
        {
            return await _assignmentService.GetAssignmentsByLessonAsync(lessonId);
        }

        [HttpGet]
        [Route("GetAssignment/{id}")]
        public async Task<IActionResult> GetAssignment(int id)
        {
            return await _assignmentService.GetAssignmentByIdAsync(id);
        }

        [HttpPost]
        [Route("CreateAssignment")]
        [RoleAuthorized(2, 3)]
        public async Task<IActionResult> CreateAssignment([FromBody] AssignmentRequest request)
        {
            return await _assignmentService.CreateAssignmentAsync(request);
        }

        [HttpPut]
        [Route("UpdateAssignment/{id}")]
        [RoleAuthorized(2, 3)]
        public async Task<IActionResult> UpdateAssignment(int id, [FromBody] AssignmentRequest request)
        {
            return await _assignmentService.UpdateAssignmentAsync(id, request);
        }

        [HttpDelete]
        [Route("DeleteAssignment/{id}")]
        [RoleAuthorized(3)]
        public async Task<IActionResult> DeleteAssignment(int id)
        {
            return await _assignmentService.DeleteAssignmentAsync(id);
        }
    }
}