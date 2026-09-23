using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldSkillsAPI.Models
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Assignment")]
        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public UserAccount User { get; set; }

        public string Content { get; set; }
        public string FileUrl { get; set; }
        public DateTime SubmittedAt { get; set; }

        [ForeignKey("Status")]
        public int StatusId { get; set; }
        public AnswerStatus Status { get; set; }
    }
}
