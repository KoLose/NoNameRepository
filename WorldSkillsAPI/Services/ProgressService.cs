using WorldSkillsAPI.DatabaseContext;
using WorldSkillsAPI.Interfaces;
using WorldSkillsAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WorldSkillsAPI.Services
{
    public class ProgressService : IProgressService
    {
        private readonly ContextDb _context;

        public ProgressService(ContextDb context)
        {
            _context = context;
        }

        public Task<IActionResult> CompleteLessonAsync(int userId, int lessonId)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> GetCourseProgressAsync(int userId, int courseId)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> GetUserProgressAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> StartLessonAsync(int userId, int lessonId)
        {
            throw new NotImplementedException();
        }
    }
}