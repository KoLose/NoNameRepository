using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace WorldSkillsAPI.Interfaces
{
    public interface IAssignmentService
    {
        Task<IActionResult> GetAssignmentsByLessonAsync(int lessonId);    // задания урока
        Task<IActionResult> GetAssignmentByIdAsync(int id);               // задание по id
        Task<IActionResult> CreateAssignmentAsync(AssignmentRequest request); // создать задание
        Task<IActionResult> UpdateAssignmentAsync(int id, AssignmentRequest request); // обновить задание
        Task<IActionResult> DeleteAssignmentAsync(int id);                // удалить задание
    }
}