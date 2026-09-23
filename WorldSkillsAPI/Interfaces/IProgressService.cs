using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace WorldSkillsAPI.Interfaces
{
    public interface IProgressService
    {
        Task<IActionResult> GetUserProgressAsync(int userId);              // весь прогресс пользователя
        Task<IActionResult> GetCourseProgressAsync(int userId, int courseId); // прогресс по курсу
        Task<IActionResult> StartLessonAsync(int userId, int lessonId);    // начать урок
        Task<IActionResult> CompleteLessonAsync(int userId, int lessonId); // завершить урок
    }
}