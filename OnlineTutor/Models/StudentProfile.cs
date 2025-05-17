using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models
{
    /// <summary>
    /// Профиль ученика
    /// </summary>
    public class StudentProfile
    {
        /// <summary>
        /// Идентификатор профиля
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор пользователя, которому принадлежит профиль
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Навигационное свойство - связанный пользователь
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Класс ученика (например, "5А", "11Б")
        /// </summary>
        [Required(ErrorMessage = "Класс обязателен для заполнения")]
        [Display(Name = "Класс")]
        [StringLength(10, ErrorMessage = "Класс не может быть длиннее 10 символов")]
        public string Grade { get; set; }

        /// <summary>
        /// Дата рождения ученика
        /// </summary>
        [Required(ErrorMessage = "Дата рождения обязательна для заполнения")]
        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Возраст ученика (вычисляемое свойство)
        /// </summary>
        [Display(Name = "Возраст")]
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        /// <summary>
        /// Идентификатор класса (учебной группы), к которому относится ученик
        /// </summary>
        public int? ClassId { get; set; }

        /// <summary>
        /// Навигационное свойство - связанный класс (учебная группа)
        /// </summary>
        public virtual Class? Class { get; set; }
    }
}
