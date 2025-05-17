using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models.ViewModels
{
    public class StudentDetailsViewModel
    {
        public int Id { get; set; }

        [Display(Name = "ФИО")]
        public string FullName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Класс")]
        public string Grade { get; set; }

        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Возраст")]
        public int Age { get; set; }

        [Display(Name = "Учебная группа")]
        public string ClassName { get; set; }
    }
}
