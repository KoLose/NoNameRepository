using WorldSkillsAPI.DatabaseContext;
using WorldSkillsAPI.Interfaces;
using WorldSkillsAPI.Models;
using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorldSkillsAPI.UniversalMethods;
using System.Net.Mail;

namespace WorldSkillsAPI.Services
{
    public class UserService : IUserService
    {
        private readonly ContextDb _context;
        private readonly jwtGenerator _jwtGenerator;

        public UserService(ContextDb context, jwtGenerator jwtGenerator)
        {
            _context = context;
            _jwtGenerator = jwtGenerator;
        }

        public async Task<IActionResult> RegisterAsync(UserRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Login))
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Логин не может быть пустым"
                    });
                }

                if (request.Password.Length < 6)
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Пароль должен содержать не менее 6 символов"
                    });
                }

                var existingUser = await _context.UserAccounts.FirstOrDefaultAsync(c =>
                    c.Login.ToLower() == request.Login.ToLower());

                if (existingUser != null)
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Пользователь с таким адресом уже зарегистрирован"
                    });
                }

                var user = new UserAccount()
                {
                    Login = request.Login,
                    Password = request.Password,
                    Name = request.Name,
                    RoleId = 1
                };

                await _context.UserAccounts.AddAsync(user);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new { status = true });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult($"Ошибка при регистрации: {ex.Message}");
            }
        }

        public async Task<IActionResult> LoginAsync(AuthUserRequest request)
        {
            var user = await _context.UserAccounts
                .FirstOrDefaultAsync(u => u.Login == request.Login && u.Password == request.Password);

            if (user == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Пользователь не найден."
                });
            }

            string token = _jwtGenerator.GenerateToken(new LoginPassword()
            {
                UserId = user.Id,
                RoleId = user.RoleId
            });

            var oldSessions = _context.Sessions.Where(s => s.UserId == user.Id);
            _context.Sessions.RemoveRange(oldSessions);

            var now = DateTime.UtcNow;
            await _context.Sessions.AddAsync(new Session
            {
                Token = token,
                UserId = user.Id,
                CreatedAt = now,
                ExpiresAt = now.AddHours(24)
            });

            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                token
            });
        }

        public async Task<IActionResult> LogoutAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Токен не передан"
                });
            }

            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Token == token);

            if (session == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Сессия не найдена"
                });
            }

            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Вы вышли из системы"
            });
        }

        public async Task<IActionResult> GetProfileAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Токен не передан"
                });
            }

            var session = await _context.Sessions
                .Include(s => s.User)
                .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(s => s.Token == token);

            if (session == null)
            {
                return new NotFoundObjectResult(new
                {
                    status = false,
                    message = "Сессия не найдена"
                });
            }

            return new OkObjectResult(new
            {
                status = true,
                user = new
                {
                    id = session.User.Id,
                    login = session.User.Login,
                    name = session.User.Name,
                    roleId = session.User.RoleId,
                    roleName = session.User.Role != null ? session.User.Role.Name : ""
                }
            });
        }

        public async Task<IActionResult> UpdateProfileAsync(
            string token,
            UpdateProfileRequest request)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return new UnauthorizedObjectResult(new
                {
                    status = false,
                    message = "Сессия не найдена"
                });
            }

            var session = await _context.Sessions
                .Include(item => item.User)
                .FirstOrDefaultAsync(item => item.Token == token);

            if (session == null)
            {
                return new UnauthorizedObjectResult(new
                {
                    status = false,
                    message = "Сессия не найдена"
                });
            }

            var login = (request.Login ?? "").Trim();
            var name = (request.Name ?? "").Trim();
            var currentPassword = request.CurrentPassword ?? "";
            var newPassword = request.NewPassword ?? "";

            if (string.IsNullOrWhiteSpace(name))
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Имя не может быть пустым"
                });
            }

            if (name.Length > 150)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Имя не должно превышать 150 символов"
                });
            }

            if (string.IsNullOrWhiteSpace(login) ||
                login.Length > 200 ||
                !MailAddress.TryCreate(login, out var email) ||
                !email.Address.Equals(login, StringComparison.OrdinalIgnoreCase))
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Введите корректный email"
                });
            }

            var loginIsTaken = await _context.UserAccounts.AnyAsync(user =>
                user.Id != session.UserId && user.Login.ToLower() == login.ToLower());
            if (loginIsTaken)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Пользователь с таким email уже зарегистрирован"
                });
            }

            if (!string.IsNullOrEmpty(currentPassword) && string.IsNullOrEmpty(newPassword))
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = "Введите новый пароль"
                });
            }

            if (!string.IsNullOrEmpty(newPassword))
            {
                if (currentPassword != session.User.Password)
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Текущий пароль указан неверно"
                    });
                }

                if (newPassword.Length < 6)
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Новый пароль должен содержать не менее 6 символов"
                    });
                }

                if (newPassword.Length > 200)
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Новый пароль не должен превышать 200 символов"
                    });
                }

                session.User.Password = newPassword;
            }

            session.User.Login = login;
            session.User.Name = name;
            await _context.SaveChangesAsync();

            return new OkObjectResult(new
            {
                status = true,
                message = "Профиль обновлён"
            });
        }
    }
}
