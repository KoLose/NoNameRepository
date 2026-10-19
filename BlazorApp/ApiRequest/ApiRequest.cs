using BlazorApp.ApiRequest.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace BlazorApp.ApiRequest
{
    public class ApiRequest
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ApiRequest(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private void SetToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            if (!string.IsNullOrWhiteSpace(token))
                _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
        }

        private async Task<T> ReadAsync<T>(HttpResponseMessage response) where T : new()
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(responseContent))
                return new T();

            return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions) ?? new T();
        }

        public async Task<StatusData> RegUser(RegUser user)
        {
            var url = "RegUser";
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, user);
                return await ReadAsync<StatusData>(response);
            }
            catch
            {
                return new StatusData();
            }
        }

        public async Task<AuthUserData> AuthUser(AuthUser user)
        {
            var url = "AuthUser";
            try
            {
                var response = await _httpClient.PostAsJsonAsync(url, user);
                return await ReadAsync<AuthUserData>(response);
            }
            catch
            {
                return new AuthUserData();
            }
        }

        public async Task<StatusData> Logout(string token)
        {
            var url = "Logout";
            try
            {
                SetToken(token);
                var response = await _httpClient.PostAsync(url, null);
                return await ReadAsync<StatusData>(response);
            }
            catch
            {
                return new StatusData();
            }
        }

        public async Task<UserData> Profile(string token)
        {
            var url = "Profile";
            try
            {
                SetToken(token);
                var response = await _httpClient.GetAsync(url);
                return await ReadAsync<UserData>(response);
            }
            catch
            {
                return new UserData();
            }
        }

        public async Task<CourseData> GetCourses()
        {
            var url = "GetCourses";
            try
            {
                var response = await _httpClient.GetAsync(url);
                return await ReadAsync<CourseData>(response);
            }
            catch
            {
                return new CourseData();
            }
        }

        public async Task<CourseData> GetCourse(int id)
        {
            var url = $"GetCourse/{id}";
            try
            {
                var response = await _httpClient.GetAsync(url);
                return await ReadAsync<CourseData>(response);
            }
            catch
            {
                return new CourseData();
            }
        }

        public async Task<CreatedIdData> CreateCourse(CourseDataShort course, string token)
        {
            var url = "CreateCourse";
            try
            {
                SetToken(token);
                var response = await _httpClient.PostAsJsonAsync(url, course);
                return await ReadAsync<CreatedIdData>(response);
            }
            catch
            {
                return new CreatedIdData();
            }
        }

        public async Task<StatusData> UpdateCourse(int id, CourseDataShort course, string token)
        {
            var url = $"UpdateCourse/{id}";
            try
            {
                SetToken(token);
                var response = await _httpClient.PutAsJsonAsync(url, course);
                return await ReadAsync<StatusData>(response);
            }
            catch
            {
                return new StatusData();
            }
        }

        public async Task<StatusData> DeleteCourse(int id, string token)
        {
            var url = $"DeleteCourse/{id}";
            try
            {
                SetToken(token);
                var response = await _httpClient.DeleteAsync(url);
                return await ReadAsync<StatusData>(response);
            }
            catch
            {
                return new StatusData();
            }
        }

        public async Task<FavoriteData> GetFavorites(string token)
        {
            var url = "Favorites";
            try
            {
                SetToken(token);
                var response = await _httpClient.GetAsync(url);
                return await ReadAsync<FavoriteData>(response);
            }
            catch
            {
                return new FavoriteData();
            }
        }

        public async Task<StatusData> AddFavorite(int courseId, string token)
        {
            var url = $"AddFavorite/{courseId}";
            try
            {
                SetToken(token);
                var response = await _httpClient.PostAsync(url, null);
                return await ReadAsync<StatusData>(response);
            }
            catch
            {
                return new StatusData();
            }
        }

        public async Task<StatusData> RemoveFavorite(int courseId, string token)
        {
            var url = $"RemoveFavorite/{courseId}";
            try
            {
                SetToken(token);
                var response = await _httpClient.DeleteAsync(url);
                return await ReadAsync<StatusData>(response);
            }
            catch
            {
                return new StatusData();
            }
        }

        public async Task<ReminderResult> SendCourseReminder(
            int courseId,
            CourseReminderRequest reminder,
            string token)
        {
            var url = $"api/Reminder/course/{courseId}/send";
            try
            {
                SetToken(token);
                var response = await _httpClient.PostAsJsonAsync(url, reminder);
                return await ReadAsync<ReminderResult>(response);
            }
            catch
            {
                return new ReminderResult();
            }
        }

        public async Task<LessonData> GetLessonsByCourse(int courseId)
        {
            var url = $"GetByCourse/{courseId}";
            try
            {
                var response = await _httpClient.GetAsync(url);
                return await ReadAsync<LessonData>(response);
            }
            catch
            {
                return new LessonData();
            }
        }

        public async Task<LessonData> GetLesson(int id)
        {
            var url = $"GetLesson/{id}";
            try
            {
                var response = await _httpClient.GetAsync(url);
                return await ReadAsync<LessonData>(response);
            }
            catch
            {
                return new LessonData();
            }
        }

        public async Task<CreatedIdData> CreateLesson(LessonDataShort lesson, string token)
        {
            var url = "CreateLesson";
            try
            {
                SetToken(token);
                var response = await _httpClient.PostAsJsonAsync(url, lesson);
                return await ReadAsync<CreatedIdData>(response);
            }
            catch
            {
                return new CreatedIdData();
            }
        }

        public async Task<StatusData> UpdateLesson(int id, LessonDataShort lesson, string token)
        {
            var url = $"UpdateLesson/{id}";
            try
            {
                SetToken(token);
                var response = await _httpClient.PutAsJsonAsync(url, lesson);
                return await ReadAsync<StatusData>(response);
            }
            catch
            {
                return new StatusData();
            }
        }

        public async Task<StatusData> DeleteLesson(int id, string token)
        {
            var url = $"DeleteLesson/{id}";
            try
            {
                SetToken(token);
                var response = await _httpClient.DeleteAsync(url);
                return await ReadAsync<StatusData>(response);
            }
            catch
            {
                return new StatusData();
            }
        }

        public async Task<AssignmentData> GetAssignmentsByLesson(int lessonId)
        {
            var url = $"GetByLesson/{lessonId}";
            try
            {
                var response = await _httpClient.GetAsync(url);
                return await ReadAsync<AssignmentData>(response);
            }
            catch
            {
                return new AssignmentData();
            }
        }

        public async Task<AssignmentData> GetAssignment(int id)
        {
            var url = $"GetAssignment/{id}";
            try
            {
                var response = await _httpClient.GetAsync(url);
                return await ReadAsync<AssignmentData>(response);
            }
            catch
            {
                return new AssignmentData();
            }
        }

        public async Task<CreatedIdData> CreateAssignment(AssignmentDataShort assignment, string token)
        {
            var url = "CreateAssignment";
            try
            {
                SetToken(token);
                var response = await _httpClient.PostAsJsonAsync(url, assignment);
                return await ReadAsync<CreatedIdData>(response);
            }
            catch
            {
                return new CreatedIdData();
            }
        }

        public async Task<StatusData> UpdateAssignment(int id, AssignmentDataShort assignment, string token)
        {
            var url = $"UpdateAssignment/{id}";
            try
            {
                SetToken(token);
                var response = await _httpClient.PutAsJsonAsync(url, assignment);
                return await ReadAsync<StatusData>(response);
            }
            catch
            {
                return new StatusData();
            }
        }

        public async Task<StatusData> DeleteAssignment(int id, string token)
        {
            var url = $"DeleteAssignment/{id}";
            try
            {
                SetToken(token);
                var response = await _httpClient.DeleteAsync(url);
                return await ReadAsync<StatusData>(response);
            }
            catch
            {
                return new StatusData();
            }
        }

        public async Task<AnswerData> GetAnswersByAssignment(int assignmentId, string token)
        {
            var url = $"api/Answer/GetByAssignment/{assignmentId}";
            try
            {
                SetToken(token);
                var response = await _httpClient.GetAsync(url);
                return await ReadAsync<AnswerData>(response);
            }
            catch
            {
                return new AnswerData();
            }
        }

        public async Task<AnswerData> GetMyAnswers(string token)
        {
            var url = "api/Answer/MyAnswers";
            try
            {
                SetToken(token);
                var response = await _httpClient.GetAsync(url);
                return await ReadAsync<AnswerData>(response);
            }
            catch
            {
                return new AnswerData();
            }
        }

        public async Task<AnswerData> GetAnswer(int id, string token)
        {
            var url = $"api/Answer/GetAnswer/{id}";
            try
            {
                SetToken(token);
                var response = await _httpClient.GetAsync(url);
                return await ReadAsync<AnswerData>(response);
            }
            catch
            {
                return new AnswerData();
            }
        }

        public async Task<CreatedIdData> SubmitAnswer(AnswerDataShort answer, string token)
        {
            var url = "api/Answer/SubmitAnswer";
            try
            {
                SetToken(token);
                var response = await _httpClient.PostAsJsonAsync(url, answer);
                return await ReadAsync<CreatedIdData>(response);
            }
            catch
            {
                return new CreatedIdData();
            }
        }

        public async Task<StatusData> UpdateAnswer(int id, AnswerDataShort answer, string token)
        {
            var url = $"api/Answer/UpdateAnswer/{id}";
            try
            {
                SetToken(token);
                var response = await _httpClient.PutAsJsonAsync(url, answer);
                return await ReadAsync<StatusData>(response);
            }
            catch
            {
                return new StatusData();
            }
        }

        public async Task<GradeData> GetGradesByAnswer(int answerId, string token)
        {
            var url = $"GetByAnswer/{answerId}";
            try
            {
                SetToken(token);
                var response = await _httpClient.GetAsync(url);
                return await ReadAsync<GradeData>(response);
            }
            catch
            {
                return new GradeData();
            }
        }

        public async Task<GradeData> GetMyGrades(string token)
        {
            var url = "MyGrades";
            try
            {
                SetToken(token);
                var response = await _httpClient.GetAsync(url);
                return await ReadAsync<GradeData>(response);
            }
            catch
            {
                return new GradeData();
            }
        }

        public async Task<GradeData> GetMyScores(string token)
        {
            var url = "MyScores";
            try
            {
                SetToken(token);
                var response = await _httpClient.GetAsync(url);
                return await ReadAsync<GradeData>(response);
            }
            catch
            {
                return new GradeData();
            }
        }

        public async Task<CreatedIdData> CreateGrade(GradeDataShort grade, string token)
        {
            var url = "CreateGrade";
            try
            {
                SetToken(token);
                var response = await _httpClient.PostAsJsonAsync(url, grade);
                return await ReadAsync<CreatedIdData>(response);
            }
            catch
            {
                return new CreatedIdData();
            }
        }

        public async Task<StatusData> UpdateGrade(int id, GradeDataShort grade, string token)
        {
            var url = $"UpdateGrade/{id}";
            try
            {
                SetToken(token);
                var response = await _httpClient.PutAsJsonAsync(url, grade);
                return await ReadAsync<StatusData>(response);
            }
            catch
            {
                return new StatusData();
            }
        }

        public async Task<ProgressData> GetMyProgress(string token)
        {
            var url = "MyProgress";
            try
            {
                SetToken(token);
                var response = await _httpClient.GetAsync(url);
                return await ReadAsync<ProgressData>(response);
            }
            catch
            {
                return new ProgressData();
            }
        }

        public async Task<ProgressData> GetCourseProgress(int courseId, string token)
        {
            var url = $"CourseProgress/{courseId}";
            try
            {
                SetToken(token);
                var response = await _httpClient.GetAsync(url);
                return await ReadAsync<ProgressData>(response);
            }
            catch
            {
                return new ProgressData();
            }
        }

        public async Task<StatusData> StartLesson(int lessonId, string token)
        {
            var url = $"StartLesson/{lessonId}";
            try
            {
                SetToken(token);
                var response = await _httpClient.PostAsync(url, null);
                return await ReadAsync<StatusData>(response);
            }
            catch
            {
                return new StatusData();
            }
        }

        public async Task<StatusData> CompleteLesson(int lessonId, string token)
        {
            var url = $"CompleteLesson/{lessonId}";
            try
            {
                SetToken(token);
                var response = await _httpClient.PostAsync(url, null);
                return await ReadAsync<StatusData>(response);
            }
            catch
            {
                return new StatusData();
            }
        }

        public async Task<CommentData> GetComments(int lessonId, string token)
        {
            var url = $"Comments/{lessonId}";
            try
            {
                SetToken(token);
                var response = await _httpClient.GetAsync(url);
                return await ReadAsync<CommentData>(response);
            }
            catch
            {
                return new CommentData();
            }
        }

        public async Task<CommentData> AddComment(int lessonId, CommentRequest comment, string token)
        {
            var url = $"AddComment/{lessonId}";
            try
            {
                SetToken(token);
                var response = await _httpClient.PostAsJsonAsync(url, comment);
                return await ReadAsync<CommentData>(response);
            }
            catch
            {
                return new CommentData();
            }
        }

        public async Task<DirectoryData> GetRoles(string token)
        {
            return await GetDirectory("Roles", token);
        }

        public async Task<DirectoryData> GetProgressStatuses(string token)
        {
            return await GetDirectory("ProgressStatuses", token);
        }

        public async Task<DirectoryData> GetAnswerStatuses(string token)
        {
            return await GetDirectory("AnswerStatuses", token);
        }

        public async Task<DirectoryData> GetPeriodicities(string token)
        {
            return await GetDirectory("Periodicities", token);
        }

        private async Task<DirectoryData> GetDirectory(string url, string token)
        {
            try
            {
                SetToken(token);
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                var items = JsonSerializer.Deserialize<List<DirectoryItem>>(responseContent, _jsonOptions) ?? new();
                return new DirectoryData
                {
                    status = true,
                    items = items
                };
            }
            catch
            {
                return new DirectoryData();
            }
        }
    }
}
