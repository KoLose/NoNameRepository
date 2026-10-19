namespace BlazorApp.ApiRequest.Models
{
    public class RegUser
    {
        public string login { get; set; } = "";
        public string password { get; set; } = "";
        public string name { get; set; } = "";
    }

    public class AuthUser
    {
        public string login { get; set; } = "";
        public string password { get; set; } = "";
    }

    public class AuthUserData
    {
        public bool status { get; set; }
        public string message { get; set; } = "";
        public string token { get; set; } = "";
    }

    public class UpdateProfileRequest
    {
        public string login { get; set; } = "";
        public string name { get; set; } = "";
        public string currentPassword { get; set; } = "";
        public string newPassword { get; set; } = "";
    }

    public class UserDataShort
    {
        public int id { get; set; }
        public string login { get; set; } = "";
        public string name { get; set; } = "";
        public int roleId { get; set; }
        public string roleName { get; set; } = "";
    }

    public class UserData
    {
        public bool status { get; set; }
        public string message { get; set; } = "";
        public UserDataShort user { get; set; } = new();
    }
}
