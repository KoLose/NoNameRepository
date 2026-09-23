using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace WorldSkillsAPI.Interfaces
{
    public interface IGradeService
    {
        Task<IActionResult> GetGradesByAnswerAsync(int answerId);          // оценки ответа
        Task<IActionResult> GetGradesByMentorAsync(int mentorId);          // оценки, выставленные наставником
        Task<IActionResult> CreateGradeAsync(GradeRequest request, int mentorId); // выставить оценку
        Task<IActionResult> UpdateGradeAsync(int id, GradeRequest request); // изменить оценку
    }
}