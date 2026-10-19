using WorldSkillsAPI.DatabaseContext;
using WorldSkillsAPI.Interfaces;
using WorldSkillsAPI.Models;
using WorldSkillsAPI.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WorldSkillsAPI.Services
{
    public class CourseService : ICourseService
    {
        private const int RoleMentor = 2;
        private const int RoleAdmin = 3;

        private readonly ContextDb _context;

        public CourseService(ContextDb context)
        {
            _context = context;
        }

        private async Task<int?> GetUserIdByTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            var session = await _context.Sessions
                .FirstOrDefaultAsync(s => s.Token == token);

            return session?.UserId;
        }

        private async Task<(int UserId, int RoleId)?> GetActorAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var session = await _context.Sessions
                .Where(item => item.Token == token)
                .Select(item => new
                {
                    item.UserId,
                    item.User.RoleId
                })
                .FirstOrDefaultAsync();

            return session == null ? null : (session.UserId, session.RoleId);
        }

        public async Task<IActionResult> GetCoursesAsync()
        {
            try
            {
                var courses = await _context.Courses
                    .Include(c => c.Mentor)
                    .Select(c => new
                    {
                        id = c.Id,
                        name = c.Name,
                        description = c.Description,
                        mentorId = c.MentorId,
                        mentorName = c.Mentor != null ? c.Mentor.Name : null
                    })
                    .ToListAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    courses
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при получении курсов: {ex.Message}"
                });
            }
        }

        public async Task<IActionResult> GetCourseByIdAsync(int id)
        {
            try
            {
                var course = await _context.Courses
                    .Include(c => c.Mentor)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (course == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Курс не найден"
                    });
                }

                var lessons = await _context.Lessons
                    .Where(l => l.CourseId == id)
                    .OrderBy(l => l.Position)
                    .Select(l => new
                    {
                        id = l.Id,
                        name = l.Name,
                        position = l.Position
                    })
                    .ToListAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    course = new
                    {
                        id = course.Id,
                        name = course.Name,
                        description = course.Description,
                        mentorId = course.MentorId,
                        mentorName = course.Mentor != null ? course.Mentor.Name : null,
                        lessons
                    }
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при получении курса: {ex.Message}"
                });
            }
        }

        public async Task<IActionResult> CreateCourseAsync(CourseRequest request, string token)
        {
            try
            {
                var actor = await GetActorAsync(token);
                if (actor == null)
                {
                    return new UnauthorizedObjectResult(new
                    {
                        status = false,
                        message = "Сессия не найдена"
                    });
                }

                if (actor.Value.RoleId != RoleMentor && actor.Value.RoleId != RoleAdmin)
                {
                    return AccessDenied("Нет прав на создание курса");
                }

                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Название курса обязательно"
                    });
                }

                var mentorId = actor.Value.RoleId == RoleMentor
                    ? actor.Value.UserId
                    : request.MentorId;

                var mentor = await _context.UserAccounts
                    .FirstOrDefaultAsync(user =>
                        user.Id == mentorId && user.RoleId == RoleMentor);

                if (mentor == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Наставник не найден или пользователь не является наставником"
                    });
                }

                var course = new Course
                {
                    Name = request.Name,
                    Description = request.Description,
                    MentorId = mentorId
                };

                await _context.Courses.AddAsync(course);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    courseId = course.Id
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при создании курса: {ex.Message}"
                });
            }
        }

        public async Task<IActionResult> UpdateCourseAsync(int id, CourseRequest request, string token)
        {
            try
            {
                var actor = await GetActorAsync(token);
                if (actor == null)
                {
                    return new UnauthorizedObjectResult(new
                    {
                        status = false,
                        message = "Сессия не найдена"
                    });
                }

                if (actor.Value.RoleId != RoleMentor && actor.Value.RoleId != RoleAdmin)
                {
                    return AccessDenied("Нет прав на изменение курса");
                }

                var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
                if (course == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Курс не найден"
                    });
                }

                if (actor.Value.RoleId == RoleMentor && course.MentorId != actor.Value.UserId)
                {
                    return AccessDenied("Можно изменять только свои курсы");
                }

                if (!string.IsNullOrWhiteSpace(request.Name))
                    course.Name = request.Name;

                if (request.Description != null)
                    course.Description = request.Description;

                if (actor.Value.RoleId == RoleAdmin && request.MentorId > 0)
                {
                    var mentorExists = await _context.UserAccounts.AnyAsync(user =>
                        user.Id == request.MentorId && user.RoleId == RoleMentor);
                    if (!mentorExists)
                    {
                        return new BadRequestObjectResult(new
                        {
                            status = false,
                            message = "Наставник не найден или пользователь не является наставником"
                        });
                    }

                    course.MentorId = request.MentorId;
                }

                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    message = "Курс обновлён"
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при обновлении курса: {ex.Message}"
                });
            }
        }

        public async Task<IActionResult> DeleteCourseAsync(int id)
        {
            try
            {
                var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);
                if (course == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Курс не найден"
                    });
                }

                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    message = "Курс удалён"
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при удалении курса: {ex.Message}"
                });
            }
        }

        public async Task<IActionResult> GetFavoriteCoursesAsync(string token)
        {
            try
            {
                var userId = await GetUserIdByTokenAsync(token);
                if (userId == null)
                {
                    return new UnauthorizedObjectResult(new
                    {
                        status = false,
                        message = "Сессия не найдена"
                    });
                }

                var favorites = await _context.FavoriteCourses
                    .Include(f => f.Course)
                    .Where(f => f.UserId == userId.Value)
                    .Select(f => new
                    {
                        id = f.Id,
                        courseId = f.CourseId,
                        courseName = f.Course != null ? f.Course.Name : null,
                        description = f.Course != null ? f.Course.Description : null,
                        addedAt = f.FavoritedAt
                    })
                    .ToListAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    favorites
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при получении избранного: {ex.Message}"
                });
            }
        }

        public async Task<IActionResult> AddFavoriteCourseAsync(string token, int courseId)
        {
            try
            {
                var userId = await GetUserIdByTokenAsync(token);
                if (userId == null)
                {
                    return new UnauthorizedObjectResult(new
                    {
                        status = false,
                        message = "Сессия не найдена"
                    });
                }

                var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
                if (course == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Курс не найден"
                    });
                }

                var existing = await _context.FavoriteCourses
                    .FirstOrDefaultAsync(f => f.UserId == userId.Value && f.CourseId == courseId);

                if (existing != null)
                {
                    return new BadRequestObjectResult(new
                    {
                        status = false,
                        message = "Курс уже в избранном"
                    });
                }

                var favorite = new FavoriteCourse
                {
                    UserId = userId.Value,
                    CourseId = courseId,
                    FavoritedAt = DateTime.UtcNow
                };

                await _context.FavoriteCourses.AddAsync(favorite);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    message = "Курс добавлен в избранное"
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при добавлении в избранное: {ex.Message}"
                });
            }
        }

        public async Task<IActionResult> RemoveFavoriteCourseAsync(string token, int courseId)
        {
            try
            {
                var userId = await GetUserIdByTokenAsync(token);
                if (userId == null)
                {
                    return new UnauthorizedObjectResult(new
                    {
                        status = false,
                        message = "Сессия не найдена"
                    });
                }

                var favorite = await _context.FavoriteCourses
                    .FirstOrDefaultAsync(f => f.UserId == userId.Value && f.CourseId == courseId);

                if (favorite == null)
                {
                    return new NotFoundObjectResult(new
                    {
                        status = false,
                        message = "Курс не найден в избранном"
                    });
                }

                _context.FavoriteCourses.Remove(favorite);
                await _context.SaveChangesAsync();

                return new OkObjectResult(new
                {
                    status = true,
                    message = "Курс убран из избранного"
                });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = false,
                    message = $"Ошибка при удалении из избранного: {ex.Message}"
                });
            }
        }

        private static ObjectResult AccessDenied(string message) => new(new
        {
            status = false,
            message
        })
        { StatusCode = StatusCodes.Status403Forbidden };
    }
}
