using WorldSkillsAPI.DatabaseContext;
using WorldSkillsAPI.Interfaces;
using WorldSkillsAPI.Models;
using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WorldSkillsAPI.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly ContextDb _context;

        public AssignmentService(ContextDb context)
        {
            _context = context;
        }

        public Task<IActionResult> CreateAssignmentAsync(AssignmentRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> DeleteAssignmentAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> GetAssignmentByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> GetAssignmentsByLessonAsync(int lessonId)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> UpdateAssignmentAsync(int id, AssignmentRequest request)
        {
            throw new NotImplementedException();
        }
    }
}