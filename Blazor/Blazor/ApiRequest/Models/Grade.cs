namespace BlazorApp.ApiRequest.Models
{
    public class GradeDataShort
    {
        public int answerId { get; set; }
        public int score { get; set; }
    }

    public class GradeItem
    {
        public int id { get; set; }
        public int answerId { get; set; }
        public int score { get; set; }
        public DateTime? gradedAt { get; set; }
        public int mentorId { get; set; }
        public string mentorName { get; set; } = "";
        public int assignmentId { get; set; }
        public string assignmentName { get; set; } = "";
    }

    public class GradeData
    {
        public bool status { get; set; }
        public string message { get; set; } = "";
        public List<GradeItem> grades { get; set; } = new();
    }
}
