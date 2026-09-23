using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldSkillsAPI.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }


        [ForeignKey("Mentor")]
        public int MentorId { get; set; }
        public UserAccount Mentor { get; set; }
    }
}
