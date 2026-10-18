namespace BlazorApp.ApiRequest.Models
{
    public class AssignmentDataShort
    {
        public int lessonId { get; set; }
        public string name { get; set; } = "";
        public string description { get; set; } = "";
        public int maxScore { get; set; }
        public int position { get; set; }
    }

    public class AssignmentItem
    {
        public int id { get; set; }
        public string name { get; set; } = "";
        public string description { get; set; } = "";
        public int maxScore { get; set; }
        public int position { get; set; }
        public int lessonId { get; set; }
        public string lessonName { get; set; } = "";
    }

    public class AssignmentData
    {
        public bool status { get; set; }
        public string message { get; set; } = "";
        public List<AssignmentItem> assignments { get; set; } = new();
        public AssignmentItem assignment { get; set; } = new();
    }
}
