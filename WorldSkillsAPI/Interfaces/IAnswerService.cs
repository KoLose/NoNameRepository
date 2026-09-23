using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace WorldSkillsAPI.Interfaces
{
    public interface IAnswerService
    {
        Task<IActionResult> GetAnswersByAssignmentAsync(int assignmentId); // ответы по заданию
        Task<IActionResult> GetAnswersByUserAsync(int userId);             // ответы пользователя
        Task<IActionResult> GetAnswerByIdAsync(int id);                    // ответ по id
        Task<IActionResult> SubmitAnswerAsync(AnswerRequest request, int userId); // отправить ответ
        Task<IActionResult> UpdateAnswerAsync(int id, AnswerRequest request);     // обновить ответ
    }
}