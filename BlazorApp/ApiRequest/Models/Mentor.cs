namespace BlazorApp.ApiRequest.Models;

public class MentorStudentItem
{
    public int studentId { get; set; }
    public string name { get; set; } = "";
    public string email { get; set; } = "";
    public int startedLessons { get; set; }
    public int completedLessons { get; set; }
    public int percent { get; set; }
    public bool isCompleted { get; set; }
    public string statusName { get; set; } = "";
}

public class MentorCourseItem
{
    public int id { get; set; }
    public string name { get; set; } = "";
    public string description { get; set; } = "";
    public int totalLessons { get; set; }
    public List<MentorStudentItem> students { get; set; } = new();
}

public class MentorCourseData
{
    public bool status { get; set; }
    public string message { get; set; } = "";
    public List<MentorCourseItem> courses { get; set; } = new();
}
