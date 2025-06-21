using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models
{
    public class SpellingTest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название теста обязательно")]
        [Display(Name = "Название теста")]
        public string Title { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Display(Name = "Инструкции")]
        public string Instructions { get; set; }

        [Required]
        [Display(Name = "Дата создания")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Время на выполнение (минуты)")]
        public int? TimeLimit { get; set; }

        [Display(Name = "Опубликован")]
        public bool IsPublished { get; set; }

        [Required]
        public int TeacherId { get; set; }

        public virtual User Teacher { get; set; }
        public virtual ICollection<SpellingWord> Words { get; set; } = new List<SpellingWord>();
        public virtual ICollection<SpellingTestAssignment> Assignments { get; set; } = new List<SpellingTestAssignment>();
    }
}
