namespace BlazorApp.ApiRequest.Models
{
    public class ProgressItem
    {
        public int lessonId { get; set; }
        public string lessonName { get; set; } = "";
        public int courseId { get; set; }
        public int statusId { get; set; }
        public string statusName { get; set; } = "";
        public DateTime? startedAt { get; set; }
        public DateTime? completedAt { get; set; }
    }

    public class ProgressData
    {
        public bool status { get; set; }
        public string message { get; set; } = "";
        public int courseId { get; set; }
        public string courseName { get; set; } = "";
        public int totalLessons { get; set; }
        public int completedLessons { get; set; }
        public int percent { get; set; }
        public List<ProgressItem> progress { get; set; } = new();
    }
}
