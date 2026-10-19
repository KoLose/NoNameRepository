using WorldSkillsAPI.Interfaces;
using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace WorldSkillsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnswerController : Controller
    {
        private readonly IAnswerService _answerService;

        public AnswerController(IAnswerService answerService)
        {
            _answerService = answerService;
        }

        [HttpGet]
        [Route("GetByAssignment/{assignmentId}")]
        [RoleAuthorized(2, 3)]
        public async Task<IActionResult> GetByAssignment(int assignmentId)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault() ?? "";
            return await _answerService.GetAnswersByAssignmentAsync(assignmentId, token);
        }

        [HttpGet]
        [Route("MyAnswers")]
        [RoleAuthorized(1)]
        public async Task<IActionResult> MyAnswers()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault();
            return await _answerService.GetAnswersByUserAsync(token);
        }

        [HttpGet]
        [Route("GetAnswer/{id}")]
        [RoleAuthorized]
        public async Task<IActionResult> GetAnswer(int id)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault();
            return await _answerService.GetAnswerByIdAsync(id, token);
        }

        [HttpPost]
        [Route("SubmitAnswer")]
        [RoleAuthorized(1)]
        public async Task<IActionResult> SubmitAnswer([FromBody] AnswerRequest request)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault();
            return await _answerService.SubmitAnswerAsync(request, token);
        }

        [HttpPut]
        [Route("UpdateAnswer/{id}")]
        [RoleAuthorized(1)]
        public async Task<IActionResult> UpdateAnswer(int id, [FromBody] AnswerRequest request)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault();
            return await _answerService.UpdateAnswerAsync(id, request, token);
        }
    }
}
