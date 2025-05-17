using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models
{
    public class Question
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Текст вопроса обязателен")]
        [Display(Name = "Текст вопроса")]
        public string Text { get; set; }

        [Display(Name = "Тип вопроса")]
        public QuestionType Type { get; set; }

        [Display(Name = "Баллы")]
        [Range(1, 100, ErrorMessage = "Баллы должны быть от 1 до 100")]
        public int Points { get; set; } = 1;

        [Display(Name = "Порядок вопроса")]
        public int OrderIndex { get; set; }

        // Внешний ключ
        public int TestId { get; set; }

        // Навигационные свойства
        public virtual Test Test { get; set; }
        public virtual ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    }

    public enum QuestionType
    {
        [Display(Name = "Один из многих")]
        SingleChoice,

        [Display(Name = "Многие из многих")]
        MultipleChoice,

        [Display(Name = "Короткий ответ")]
        ShortAnswer,

        [Display(Name = "Развернутый ответ")]
        Essay
    }
}
