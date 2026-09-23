namespace WorldSkillsAPI.Requests
{
    public class AssignmentRequest
    {
        public int LessonId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int MaxScore { get; set; }
        public int Position { get; set; }
    }
}
