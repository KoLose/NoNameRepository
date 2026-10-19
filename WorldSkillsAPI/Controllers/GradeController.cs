using WorldSkillsAPI.Interfaces;
using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace WorldSkillsAPI.Controllers
{
    [ApiController]
    public class GradeController : Controller
    {
        private readonly IGradeService _gradeService;

        public GradeController(IGradeService gradeService)
        {
            _gradeService = gradeService;
        }

        [HttpGet]
        [Route("GetByAnswer/{answerId}")]
        [RoleAuthorized(2, 3)]
        public async Task<IActionResult> GetByAnswer(int answerId)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault() ?? "";
            return await _gradeService.GetGradesByAnswerAsync(answerId, token);
        }

        [HttpGet]
        [Route("MyGrades")]
        [RoleAuthorized(2, 3)]
        public async Task<IActionResult> MyGrades()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault();
            return await _gradeService.GetGradesByMentorAsync(token);
        }

        [HttpGet]
        [Route("MyScores")]
        [RoleAuthorized]
        public async Task<IActionResult> MyScores()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault();
            return await _gradeService.GetMyGradesAsync(token);
        }

        [HttpPost]
        [Route("CreateGrade")]
        [RoleAuthorized(2, 3)]
        public async Task<IActionResult> CreateGrade([FromBody] GradeRequest request)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault();
            return await _gradeService.CreateGradeAsync(request, token);
        }

        [HttpPut]
        [Route("UpdateGrade/{id}")]
        [RoleAuthorized(2, 3)]
        public async Task<IActionResult> UpdateGrade(int id, [FromBody] GradeRequest request)
        {
            var token = Request.Headers["Authorization"].FirstOrDefault();
            return await _gradeService.UpdateGradeAsync(id, request, token);
        }
    }
}
