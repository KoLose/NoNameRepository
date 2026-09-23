using WorldSkillsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace WorldSkillsAPI.DatabaseContext
{
    public class ContextDb : DbContext
    {
        public ContextDb(DbContextOptions<ContextDb> options) : base(options)
        {

        }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<ProgressStatus> ProgressStatuses { get; set; }
        public DbSet<AnswerStatus> AnswerStatuses { get; set; }
        public DbSet<Periodicity> Periodicities { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<FavoriteCourse> FavoriteCourses { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<LessonProgress> LessonProgresses { get; set; }
        public DbSet<Reminder> Reminders { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FavoriteCourse>()
                .HasIndex(f => new { f.UserId, f.CourseId })
                .IsUnique();

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Пользователь" },
                new Role { Id = 2, Name = "Наставник" },
                new Role { Id = 3, Name = "Администратор" }
            );

            modelBuilder.Entity<ProgressStatus>().HasData(
                new ProgressStatus { Id = 1, Name = "Не начат" },
                new ProgressStatus { Id = 2, Name = "В процессе" },
                new ProgressStatus { Id = 3, Name = "Завершён" }
            );

            modelBuilder.Entity<AnswerStatus>().HasData(
                new AnswerStatus { Id = 1, Name = "Не дан" },
                new AnswerStatus { Id = 2, Name = "На проверке" },
                new AnswerStatus { Id = 3, Name = "Принят" },
                new AnswerStatus { Id = 4, Name = "Отклонён" }
            );

            modelBuilder.Entity<Periodicity>().HasData(
                new Periodicity { Id = 1, Name = "Однократно" },
                new Periodicity { Id = 2, Name = "Ежедневно" },
                new Periodicity { Id = 3, Name = "Еженедельно" },
                new Periodicity { Id = 4, Name = "Ежемесячно" }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}