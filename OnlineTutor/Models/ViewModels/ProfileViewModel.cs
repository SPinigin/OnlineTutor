using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models.ViewModels
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Поле Имя обязательно для заполнения")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Поле Фамилия обязательно для заполнения")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Поле Email обязательно для заполнения")]
        [EmailAddress(ErrorMessage = "Некорректный формат Email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Телефон")]
        [Phone(ErrorMessage = "Некорректный номер телефона")]
        [RegularExpression(@"^(\+7|7|8)?[\s\-]?[489][0-9]{2}[\s\-]?[0-9]{3}[\s\-]?[0-9]{2}[\s\-]?[0-9]{2}$", ErrorMessage = "Некорректный формат телефона")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Роль")]
        public UserRole Role { get; set; }

        // Поля для студента
        [Display(Name = "Класс")]
        public string Grade { get; set; }

        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        // Поля для учителя
        [Display(Name = "Предмет")]
        public string Subject { get; set; }

        [Display(Name = "Опыт работы (лет)")]
        public int? TeachingExperience { get; set; }

        [Display(Name = "Образование")]
        public string Education { get; set; }
    }
}
