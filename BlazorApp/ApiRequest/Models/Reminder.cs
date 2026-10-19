namespace BlazorApp.ApiRequest.Models;

public class CourseReminderRequest
{
    public string text { get; set; } = "";
}

public class ReminderResult
{
    public bool status { get; set; }
    public string message { get; set; } = "";
    public int sentCount { get; set; }
    public int failedCount { get; set; }
}
