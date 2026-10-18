namespace BlazorApp.ApiRequest.Models
{
    public class FavoriteItem
    {
        public int id { get; set; }
        public int courseId { get; set; }
        public string courseName { get; set; } = "";
        public string description { get; set; } = "";
        public DateTime? addedAt { get; set; }
    }

    public class FavoriteData
    {
        public bool status { get; set; }
        public string message { get; set; } = "";
        public List<FavoriteItem> favorites { get; set; } = new();
    }
}
