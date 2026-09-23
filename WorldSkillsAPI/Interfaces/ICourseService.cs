using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;

namespace WorldSkillsAPI.Interfaces
{
    public interface ICourseService
    {
        Task<IActionResult> GetCoursesAsync();                            // список всех курсов
        Task<IActionResult> GetCourseByIdAsync(int id);                   // курс по id
        Task<IActionResult> CreateCourseAsync(CourseRequest request);     // создать курс
        Task<IActionResult> UpdateCourseAsync(int id, CourseRequest request); // обновить курс
        Task<IActionResult> DeleteCourseAsync(int id);                    // удалить курс
        Task<IActionResult> GetFavoriteCoursesAsync(string token);        // избранные курсы
        Task<IActionResult> AddFavoriteCourseAsync(string token, int courseId);    // добавить в избранное
        Task<IActionResult> RemoveFavoriteCourseAsync(string token, int courseId); // убрать из избранного
    }
}