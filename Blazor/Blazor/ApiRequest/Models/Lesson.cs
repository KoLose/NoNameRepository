namespace BlazorApp.ApiRequest.Models
{
    public class LessonDataShort
    {
        public string name { get; set; } = "";
        public string content { get; set; } = "";
        public string videoUrl { get; set; } = "";
        public int position { get; set; }
        public int courseId { get; set; }
    }

    public class LessonItem
    {
        public int id { get; set; }
        public string name { get; set; } = "";
        public string content { get; set; } = "";
        public string videoUrl { get; set; } = "";
        public int position { get; set; }
        public int courseId { get; set; }
        public string courseName { get; set; } = "";
    }

    public class LessonData
    {
        public bool status { get; set; }
        public string message { get; set; } = "";
        public List<LessonItem> lessons { get; set; } = new();
        public LessonItem lesson { get; set; } = new();
    }
}
