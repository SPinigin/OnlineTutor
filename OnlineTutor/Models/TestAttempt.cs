using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models
{
    public class TestAttempt
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Дата начала")]
        public DateTime StartTime { get; set; }

        [Display(Name = "Дата завершения")]
        public DateTime? EndTime { get; set; }

        [Display(Name = "Результат (%)")]
        public double? Score { get; set; }

        [Display(Name = "Статус")]
        public TestAttemptStatus Status { get; set; }

        // Внешние ключи
        [Required]
        public int TestAssignmentId { get; set; }

        public virtual TestAssignment TestAssignment { get; set; }

        [Required]
        public int StudentId { get; set; }  // Изменено с string на int

        public virtual User Student { get; set; }

        // Навигационное свойство
        public virtual ICollection<StudentAnswer> Answers { get; set; } = new List<StudentAnswer>();
    }

    public enum TestAttemptStatus
    {
        [Display(Name = "В процессе")]
        InProgress,

        [Display(Name = "Завершен")]
        Completed,

        [Display(Name = "Пропущен")]
        Abandoned,

        [Display(Name = "Просрочен")]
        TimedOut
    }
}
