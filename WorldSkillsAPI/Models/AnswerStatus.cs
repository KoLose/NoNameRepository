using System.ComponentModel.DataAnnotations;

namespace WorldSkillsAPI.Models
{
    public class AnswerStatus
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
