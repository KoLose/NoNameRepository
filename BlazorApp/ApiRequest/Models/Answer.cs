namespace BlazorApp.ApiRequest.Models
{
    public class AnswerDataShort
    {
        public int assignmentId { get; set; }
        public string content { get; set; } = "";
        public string fileUrl { get; set; } = "";
    }

    public class AnswerItem
    {
        public int id { get; set; }
        public int userId { get; set; }
        public string userName { get; set; } = "";
        public int assignmentId { get; set; }
        public string assignmentName { get; set; } = "";
        public string content { get; set; } = "";
        public string fileUrl { get; set; } = "";
        public DateTime? submittedAt { get; set; }
        public int statusId { get; set; }
        public string statusName { get; set; } = "";
    }

    public class AnswerData
    {
        public bool status { get; set; }
        public string message { get; set; } = "";
        public List<AnswerItem> answers { get; set; } = new();
        public AnswerItem answer { get; set; } = new();
    }
}
