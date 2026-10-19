namespace WorldSkillsAPI.Requests;

public class UpdateProfileRequest
{
    public string Login { get; set; } = "";
    public string Name { get; set; } = "";
    public string CurrentPassword { get; set; } = "";
    public string NewPassword { get; set; } = "";
}
