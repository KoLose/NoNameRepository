using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldSkillsAPI.Models
{
    public class Grade
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Answer")]
        public int AnswerId { get; set; }
        public Answer Answer { get; set; }

        public int Score { get; set; }
        public DateTime GradedAt { get; set; }

        [ForeignKey("Mentor")]
        public int MentorId { get; set; }
        public UserAccount Mentor { get; set; }
    }
}
