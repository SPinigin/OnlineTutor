using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineTutor.Models;

namespace OnlineTutor.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<StudentAssignment> StudentAssignments { get; set; }
        public DbSet<AssignmentMaterial> AssignmentMaterials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка отношений между моделями
            modelBuilder.Entity<Class>()
                .HasOne(c => c.Teacher)
                .WithMany()
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Class)
                .WithMany(c => c.Students)
                .HasForeignKey(u => u.ClassId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Assignment>()
                .HasOne(a => a.Class)
                .WithMany(c => c.Assignments)
                .HasForeignKey(a => a.ClassId);

            modelBuilder.Entity<StudentAssignment>()
                .HasOne(sa => sa.Student)
                .WithMany()
                .HasForeignKey(sa => sa.StudentId);

            modelBuilder.Entity<StudentAssignment>()
                .HasOne(sa => sa.Assignment)
                .WithMany(a => a.StudentAssignments)
                .HasForeignKey(sa => sa.AssignmentId);

            modelBuilder.Entity<AssignmentMaterial>()
                .HasOne(am => am.Assignment)
                .WithMany(a => a.Materials)
                .HasForeignKey(am => am.AssignmentId);
        }
    }
}
