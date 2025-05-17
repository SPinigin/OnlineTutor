using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models
{
    public class StudentAnswer
    {
        public int Id { get; set; }

        [Display(Name = "Текст ответа")]
        public string AnswerText { get; set; }

        [Display(Name = "Выбранные варианты")]
        public string SelectedOptions { get; set; } // Хранит ID выбранных вариантов, разделенных запятой

        [Display(Name = "Баллы")]
        public double? Score { get; set; }

        [Display(Name = "Комментарий учителя")]
        public string TeacherComment { get; set; }

        // Внешние ключи
        public int TestAttemptId { get; set; }
        public virtual TestAttempt TestAttempt { get; set; }

        public int QuestionId { get; set; }
        public virtual Question Question { get; set; }
    }
}
