namespace BlazorApp.ApiRequest.Models
{
    public class StatusData
    {
        public bool status { get; set; }
        public string message { get; set; } = "";
    }

    public class CreatedIdData
    {
        public bool status { get; set; }
        public string message { get; set; } = "";
        public int courseId { get; set; }
        public int lessonId { get; set; }
        public int assignmentId { get; set; }
        public int answerId { get; set; }
        public int gradeId { get; set; }
    }
}
