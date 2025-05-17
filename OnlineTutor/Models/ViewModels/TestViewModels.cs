using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models.ViewModels
{
    public class TestViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название теста обязательно")]
        [Display(Name = "Название теста")]
        public string Title { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Инструкции")]
        public string Instructions { get; set; }

        [Required(ErrorMessage = "Предмет обязателен")]
        [Display(Name = "Предмет")]
        public string Subject { get; set; }

        [Display(Name = "Тема")]
        public string Topic { get; set; }

        [Display(Name = "Время на выполнение (минуты)")]
        public int? TimeLimit { get; set; }

        [Display(Name = "Проходной балл (%)")]
        [Range(0, 100, ErrorMessage = "Проходной балл должен быть от 0 до 100")]
        public int? PassingScore { get; set; }

        [Display(Name = "Опубликован")]
        public bool IsPublished { get; set; }
    }
}
