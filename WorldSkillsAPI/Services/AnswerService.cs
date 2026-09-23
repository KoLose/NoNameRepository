using WorldSkillsAPI.DatabaseContext;
using WorldSkillsAPI.Interfaces;
using WorldSkillsAPI.Models;
using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WorldSkillsAPI.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly ContextDb _context;
        public AnswerService(ContextDb context)
        {
            _context = context;
        }

        public Task<IActionResult> GetAnswerByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> GetAnswersByAssignmentAsync(int assignmentId)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> GetAnswersByUserAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> SubmitAnswerAsync(AnswerRequest request, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> UpdateAnswerAsync(int id, AnswerRequest request)
        {
            throw new NotImplementedException();
        }
    }
}