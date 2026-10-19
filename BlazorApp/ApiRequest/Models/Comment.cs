namespace BlazorApp.ApiRequest.Models
{
    public class CommentRequest
    {
        public string content { get; set; } = "";
    }

    public class CommentItem
    {
        public int id { get; set; }
        public string content { get; set; } = "";
        public DateTime createdAt { get; set; }
        public int userId { get; set; }
        public string userName { get; set; } = "";
        public int lessonId { get; set; }
    }

    public class CommentData
    {
        public bool status { get; set; }
        public string message { get; set; } = "";
        public List<CommentItem> comments { get; set; } = new();
        public CommentItem comment { get; set; } = new();
    }
}
