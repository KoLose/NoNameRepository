using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorldSkillsAPI.Models
{
    public class Session
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public UserAccount User { get; set; }
    }
}
