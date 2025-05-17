using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models.ViewModels
{
    public class EditStudentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле Email обязательно для заполнения")]
        [EmailAddress(ErrorMessage = "Некорректный формат Email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Поле Имя обязательно для заполнения")]
        [Display(Name = "Имя")]
        [StringLength(50, ErrorMessage = "Имя не может быть длиннее 50 символов")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Поле Фамилия обязательно для заполнения")]
        [Display(Name = "Фамилия")]
        [StringLength(50, ErrorMessage = "Фамилия не может быть длиннее 50 символов")]
        public string LastName { get; set; }

        [Display(Name = "Отчество")]
        [StringLength(50, ErrorMessage = "Отчество не может быть длиннее 50 символов")]
        public string MiddleName { get; set; }

        [Display(Name = "Новый пароль")]
        [StringLength(100, ErrorMessage = "Пароль должен содержать не менее {2} и не более {1} символов.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Класс (например, 10А)")]
        [Required(ErrorMessage = "Поле Класс обязательно для заполнения")]
        [StringLength(10, ErrorMessage = "Класс не может быть длиннее 10 символов")]
        public string Grade { get; set; }

        [Required(ErrorMessage = "Дата рождения обязательна")]
        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Класс (группа)")]
        public int? ClassId { get; set; }

        public List<Class> AvailableClasses { get; set; } = new List<Class>();
    }
}
