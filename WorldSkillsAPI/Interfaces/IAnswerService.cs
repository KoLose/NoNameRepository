using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace WorldSkillsAPI.Interfaces
{
    public interface IAnswerService
    {
        Task<IActionResult> GetAnswersByAssignmentAsync(int assignmentId);          // ответы по заданию
        Task<IActionResult> GetAnswersByUserAsync(string token);                    // мои ответы
        Task<IActionResult> GetAnswerByIdAsync(int id, string token);               // ответ по id
        Task<IActionResult> SubmitAnswerAsync(AnswerRequest request, string token); // отправить ответ
        Task<IActionResult> UpdateAnswerAsync(int id, AnswerRequest request, string token); // обновить ответ
    }
}