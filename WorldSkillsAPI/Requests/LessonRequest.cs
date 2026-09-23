namespace WorldSkillsAPI.Requests
{
    public class LessonRequest
    {
        public string Name { get; set; }
        public string Content { get; set; }
        public string VideoUrl { get; set; }
        public int Position { get; set; }
        public int CourseId { get; set; }
    }
}
