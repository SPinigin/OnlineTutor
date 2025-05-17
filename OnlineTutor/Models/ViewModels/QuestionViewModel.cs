using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models.ViewModels
{
    public class QuestionViewModel
    {
        public int Id { get; set; }
        public int TestId { get; set; }

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

        public List<QuestionOptionViewModel> Options { get; set; } = new List<QuestionOptionViewModel>();
    }
}
