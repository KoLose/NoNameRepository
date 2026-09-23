using WorldSkillsAPI.DatabaseContext;
using WorldSkillsAPI.Interfaces;
using WorldSkillsAPI.Models;
using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WorldSkillsAPI.Services
{
    public class GradeService : IGradeService
    {
        private readonly ContextDb _context;

        public GradeService(ContextDb context)
        {
            _context = context;
        }

        public Task<IActionResult> CreateGradeAsync(GradeRequest request, int mentorId)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> GetGradesByAnswerAsync(int answerId)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> GetGradesByMentorAsync(int mentorId)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> UpdateGradeAsync(int id, GradeRequest request)
        {
            throw new NotImplementedException();
        }
    }
}