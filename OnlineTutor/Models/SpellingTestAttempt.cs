using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models
{
    public class SpellingTestAttempt
    {
        public int Id { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int CorrectAnswers { get; set; }

        public int TotalAnswers { get; set; }

        public double ScorePercentage => TotalAnswers > 0 ? (double)CorrectAnswers / TotalAnswers * 100 : 0;

        public string StudentAnswers { get; set; } // JSON строка с ответами

        [Required]
        public int AssignmentId { get; set; }
        public virtual SpellingTestAssignment Assignment { get; set; }

        [Required]
        public int StudentId { get; set; }
        public virtual User Student { get; set; }
    }
}
