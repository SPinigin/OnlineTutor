using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models
{
    public class QuestionOption
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Текст варианта обязателен")]
        [Display(Name = "Текст варианта")]
        public string Text { get; set; }

        [Display(Name = "Правильный вариант")]
        public bool IsCorrect { get; set; }

        // Внешний ключ
        public int QuestionId { get; set; }

        // Навигационное свойство
        public virtual Question Question { get; set; }
    }
}
