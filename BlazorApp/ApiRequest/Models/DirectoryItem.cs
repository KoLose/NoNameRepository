namespace BlazorApp.ApiRequest.Models
{
    public class DirectoryItem
    {
        public int id { get; set; }
        public string name { get; set; } = "";
    }

    public class DirectoryData
    {
        public bool status { get; set; }
        public string message { get; set; } = "";
        public List<DirectoryItem> items { get; set; } = new();
    }
}
