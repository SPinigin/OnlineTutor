using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models.ViewModels
{
    public class SpellingTestViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название теста обязательно")]
        [Display(Name = "Название теста")]
        public string Title { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Инструкции")]
        public string Instructions { get; set; }

        [Display(Name = "Время на выполнение (минуты)")]
        public int? TimeLimit { get; set; }

        [Display(Name = "Опубликован")]
        public bool IsPublished { get; set; }
    }
}
