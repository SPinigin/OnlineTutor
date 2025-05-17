using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models.ViewModels
{
    public class QuestionOptionViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Текст варианта обязателен")]
        [Display(Name = "Текст варианта")]
        public string Text { get; set; }

        [Display(Name = "Правильный вариант")]
        public bool IsCorrect { get; set; }
    }
}
