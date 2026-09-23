using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using WorldSkillsAPI.Requests;

namespace WorldSkillsAPI.Interfaces
{
    public interface IUserService
    {
        Task<IActionResult> RegisterAsync(UserRequest request);   // регистрация
        Task<IActionResult> LoginAsync(AuthUserRequest request);     // вход
        Task<IActionResult> LogoutAsync(string token); // выход
        Task<IActionResult> GetProfileAsync(string token); // профиль пользователя
    }
}
