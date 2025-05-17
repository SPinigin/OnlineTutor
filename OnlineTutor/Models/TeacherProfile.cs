using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models
{
    /// <summary>
    /// Профиль учителя
    /// </summary>
    public class TeacherProfile
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
        /// Предмет, который преподает учитель
        /// </summary>
        [Required(ErrorMessage = "Предмет обязателен для заполнения")]
        [Display(Name = "Предмет")]
        [StringLength(100, ErrorMessage = "Предмет не может быть длиннее 100 символов")]
        public string Subject { get; set; }

        /// <summary>
        /// Опыт преподавания (в годах)
        /// </summary>
        [Required(ErrorMessage = "Опыт работы обязателен для заполнения")]
        [Display(Name = "Опыт работы (лет)")]
        [Range(0, 60, ErrorMessage = "Опыт работы должен быть от 0 до 60 лет")]
        public int TeachingExperience { get; set; }

        /// <summary>
        /// Образование учителя
        /// </summary>
        [Required(ErrorMessage = "Образование обязательно для заполнения")]
        [Display(Name = "Образование")]
        [StringLength(200, ErrorMessage = "Образование не может быть длиннее 200 символов")]
        public string Education { get; set; }

        /// <summary>
        /// Дополнительная информация об учителе
        /// </summary>
        [Display(Name = "Дополнительная информация")]
        [StringLength(500, ErrorMessage = "Дополнительная информация не может быть длиннее 500 символов")]
        public string? AdditionalInfo { get; set; }
    }
}
