namespace BlazorApp.ApiRequest.Models
{
    public class CourseDataShort
    {
        public string name { get; set; } = "";
        public string description { get; set; } = "";
        public int mentorId { get; set; }
    }

    public class CourseItem
    {
        public int id { get; set; }
        public string name { get; set; } = "";
        public string description { get; set; } = "";
        public int mentorId { get; set; }
        public string mentorName { get; set; } = "";
        public List<LessonItem> lessons { get; set; } = new();
    }

    public class CourseData
    {
        public bool status { get; set; }
        public string message { get; set; } = "";
        public List<CourseItem> courses { get; set; } = new();
        public CourseItem course { get; set; } = new();
    }
}
