using System.ComponentModel.DataAnnotations;

namespace WorldSkillsAPI.Models
{
    public class ProgressStatus
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
