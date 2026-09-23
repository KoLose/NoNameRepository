using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldSkillsAPI.Models
{
    public class Reminder
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public UserAccount User { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        public string Text { get; set; }
        public DateTime RemindAt { get; set; }

        [ForeignKey("Periodicity")]
        public int PeriodicityId { get; set; }
        public Periodicity Periodicity { get; set; }

        public bool IsRead { get; set; }
        public DateTime? NextTriggerAt { get; set; }
    }
}
