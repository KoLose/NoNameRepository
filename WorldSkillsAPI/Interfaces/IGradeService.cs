using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace WorldSkillsAPI.Interfaces
{
    public interface IGradeService
    {
        Task<IActionResult> GetGradesByAnswerAsync(int answerId);                       // оценки ответа
        Task<IActionResult> GetGradesByMentorAsync(string token);                       // оценки, выставленные наставником
        Task<IActionResult> GetMyGradesAsync(string token);                             // оценки пользователя
        Task<IActionResult> CreateGradeAsync(GradeRequest request, string token);       // выставить оценку
        Task<IActionResult> UpdateGradeAsync(int id, GradeRequest request, string token); // изменить свою оценку
    }
}