using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace WorldSkillsAPI.Interfaces
{
    public interface ILessonService
    {
        Task<IActionResult> GetLessonsByCourseAsync(int courseId);        // уроки курса
        Task<IActionResult> GetLessonByIdAsync(int id);                   // урок по id
        Task<IActionResult> CreateLessonAsync(LessonRequest request);     // создать урок
        Task<IActionResult> UpdateLessonAsync(int id, LessonRequest request); // обновить урок
        Task<IActionResult> DeleteLessonAsync(int id);                    // удалить урок
    }
}