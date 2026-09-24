using Microsoft.AspNetCore.Mvc;

namespace WorldSkillsAPI.Interfaces
{
    public interface IProgressService
    {
        Task<IActionResult> GetUserProgressAsync(string token);                       // весь прогресс пользователя
        Task<IActionResult> GetCourseProgressAsync(string token, int courseId);       // прогресс по курсу
        Task<IActionResult> StartLessonAsync(string token, int lessonId);             // начать урок
        Task<IActionResult> CompleteLessonAsync(string token, int lessonId);          // завершить урок
    }
}