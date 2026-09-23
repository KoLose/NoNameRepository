using Microsoft.AspNetCore.Mvc;

namespace WorldSkillsAPI.Interfaces
{
    public interface IDirectoryService
    {
        Task<IActionResult> GetRolesAsync(); // роли
        Task<IActionResult> GetProgressStatusesAsync(); // статусы прогресса
        Task<IActionResult> GetAnswerStatusesAsync(); // статусы ответов
        Task<IActionResult> GetPeriodicitiesAsync(); // периодичности напоминаний
    }
}
