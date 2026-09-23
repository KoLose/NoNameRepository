using WorldSkillsAPI.DatabaseContext;
using WorldSkillsAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WorldSkillsAPI.Services
{
    public class DirectoryService : IDirectoryService
    {
        private readonly ContextDb _context;

        public DirectoryService(ContextDb context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetProgressStatusesAsync()
        {
            try
            {
                var statuses = await _context.ProgressStatuses.ToListAsync();
                return new OkObjectResult(statuses);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Ошибка при получении статусов прогресса: {ex.Message}");
            }
        }

        public async Task<IActionResult> GetAnswerStatusesAsync()
        {
            try
            {
                var statuses = await _context.AnswerStatuses.ToListAsync();
                return new OkObjectResult(statuses);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Ошибка при получении статусов ответов: {ex.Message}");
            }
        }

        public async Task<IActionResult> GetPeriodicitiesAsync()
        {
            try
            {
                var periodicities = await _context.Periodicities.ToListAsync();
                return new OkObjectResult(periodicities);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Ошибка при получении периодичности: {ex.Message}");
            }
        }

        public async Task<IActionResult> GetRolesAsync()
        {
            try
            {
                var roles = await _context.Roles.ToListAsync();
                return new OkObjectResult(roles);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Ошибка при получении ролей: {ex.Message}");
            }
        }
    }
}