using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldSkillsAPI.Models
{
    public class Assignment
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Lesson")]
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int MaxScore { get; set; }
        public int Position { get; set; }
    }
}
