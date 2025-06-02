using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineTutor.Models;

namespace OnlineTutor.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Наборы данных для основных моделей
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<TeacherProfile> TeacherProfiles { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<StudentAssignment> StudentAssignments { get; set; }
        public DbSet<AssignmentMaterial> AssignmentMaterials { get; set; }

        // Наборы данных для тестов
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionOption> QuestionOptions { get; set; }
        public DbSet<TestAssignment> TestAssignments { get; set; }
        public DbSet<TestAttempt> TestAttempts { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }
        public DbSet<TestTopic> TestTopics { get; set; }
        public DbSet<TestGroup> TestGroups { get; set; }

        // События календаря
        public DbSet<CalendarEvent> CalendarEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
        .HasOne(u => u.StudentProfile)
        .WithOne(sp => sp.User)
        .HasForeignKey<StudentProfile>(sp => sp.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.TeacherProfile)
                .WithOne(tp => tp.User)
                .HasForeignKey<TeacherProfile>(tp => tp.UserId);

            // Настройка отношений между классом и учителем
            modelBuilder.Entity<Class>()
                .HasOne(c => c.Teacher)
                .WithMany()
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь между студентом и классом
            modelBuilder.Entity<StudentProfile>()
                .HasOne(sp => sp.Class)
                .WithMany(c => c.Students)
                .HasForeignKey(sp => sp.ClassId)
                .OnDelete(DeleteBehavior.SetNull);

            // Настройка отношений для заданий
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Class)
                .WithMany(c => c.Assignments)
                .HasForeignKey(a => a.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь между заданием и учителем
            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Teacher)
                .WithMany()
                .HasForeignKey(a => a.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // Связь между StudentAssignment и User (Student)
            modelBuilder.Entity<StudentAssignment>()
                .HasOne(sa => sa.Student)
                .WithMany()
                .HasForeignKey(sa => sa.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentAssignment>()
                .HasOne(sa => sa.Assignment)
                .WithMany(a => a.StudentAssignments)
                .HasForeignKey(sa => sa.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AssignmentMaterial>()
                .HasOne(am => am.Assignment)
                .WithMany(a => a.Materials)
                .HasForeignKey(am => am.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка отношений для тестов
            modelBuilder.Entity<Test>()
                .HasOne(t => t.Teacher)
                .WithMany()
                .HasForeignKey(t => t.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Test)
                .WithMany(t => t.Questions)
                .HasForeignKey(q => q.TestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<QuestionOption>()
                .HasOne(o => o.Question)
                .WithMany(q => q.Options)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TestAssignment>()
                .HasOne(ta => ta.Test)
                .WithMany(t => t.TestAssignments)
                .HasForeignKey(ta => ta.TestId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TestAssignment>()
                .HasOne(ta => ta.Class)
                .WithMany()
                .HasForeignKey(ta => ta.ClassId)
                .OnDelete(DeleteBehavior.SetNull);

            // Связь между TestAssignment и User (Student)
            modelBuilder.Entity<TestAssignment>()
                .HasOne(ta => ta.Student)
                .WithMany()
                .HasForeignKey(ta => ta.StudentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<TestAttempt>()
                .HasOne(ta => ta.TestAssignment)
                .WithMany(a => a.TestAttempts)
                .HasForeignKey(ta => ta.TestAssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Связь между TestAttempt и User (Student)
            modelBuilder.Entity<TestAttempt>()
                .HasOne(ta => ta.Student)
                .WithMany()
                .HasForeignKey(ta => ta.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentAnswer>()
                .HasOne(sa => sa.TestAttempt)
                .WithMany(ta => ta.Answers)
                .HasForeignKey(sa => sa.TestAttemptId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StudentAnswer>()
                .HasOne(sa => sa.Question)
                .WithMany()
                .HasForeignKey(sa => sa.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Материалы
            modelBuilder.Entity<MaterialTopic>()
                            .HasMany(t => t.Materials)
                            .WithOne(m => m.Topic)
                            .HasForeignKey(m => m.TopicId)
                            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Material>()
                            .HasOne(m => m.Class)
                            .WithMany(c => c.Materials)
                            .HasForeignKey(m => m.ClassId)
                            .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Material>()
                            .HasOne(m => m.Student)
                            .WithMany()
                            .HasForeignKey(m => m.StudentId)
                            .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Material>()
                            .HasOne(m => m.Teacher)
                            .WithMany()
                            .HasForeignKey(m => m.TeacherId)
                            .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<MaterialTopic> MaterialTopics { get; set; }
        public DbSet<Material> Materials { get; set; }
    }
}
