using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models
{
    public class User : IdentityUser<int>
    {
        [Required(ErrorMessage = "Имя обязательно для заполнения")]
        [Display(Name = "Имя")]
        [StringLength(50, ErrorMessage = "Имя не может быть длиннее 50 символов")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилия обязательна для заполнения")]
        [Display(Name = "Фамилия")]
        [StringLength(50, ErrorMessage = "Фамилия не может быть длиннее 50 символов")]
        public string LastName { get; set; }

        [Display(Name = "Отчество")]
        [StringLength(50, ErrorMessage = "Отчество не может быть длиннее 50 символов")]
        public string? MiddleName { get; set; }

        [Required]
        [Display(Name = "Роль")]
        public UserRole Role { get; set; }

        [Display(Name = "Полное имя")]
        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(MiddleName))
                    return $"{LastName} {FirstName} {MiddleName}";
                else
                    return $"{LastName} {FirstName}";
            }
        }

        [Display(Name = "Сокращенное имя")]
        public string ShortName
        {
            get
            {
                if (!string.IsNullOrEmpty(MiddleName))
                    return $"{LastName} {FirstName[0]}. {MiddleName[0]}.";
                else
                    return $"{LastName} {FirstName[0]}.";
            }
        }

        [Display(Name = "Подтвержденный аккаунт")]
        public bool IsVerified { get; set; } = false;

        public virtual StudentProfile? StudentProfile { get; set; }
        public virtual TeacherProfile? TeacherProfile { get; set; }
    }

    public enum UserRole
    {
        [Display(Name = "Ученик")]
        Student,

        [Display(Name = "Учитель")]
        Teacher,

        [Display(Name = "Администратор")]
        Administrator
    }
}
