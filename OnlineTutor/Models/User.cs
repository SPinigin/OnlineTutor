using Microsoft.AspNetCore.Identity;

namespace OnlineTutor.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? MiddleName { get; set; }
        public UserRole Role { get; set; }

        // Поля для ученика
        public string Grade { get; set; }
        public DateTime? DateOfBirth { get; set; }

        // Поля для учителя
        public string Subject { get; set; }
        public int? TeachingExperience { get; set; }
        public string Education { get; set; }

        // Существующие поля
        public int? ClassId { get; set; }
        public virtual Class Class { get; set; }
    }

    public enum UserRole
    {
        Student,
        Teacher,
        Administrator
    }
}
