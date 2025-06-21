using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models
{
    public class SpellingWord
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Слово с пропуском")]
        public string WordWithGap { get; set; }

        [Required]
        [Display(Name = "Правильная буква")]
        public string CorrectLetter { get; set; }

        [Required]
        [Display(Name = "Полное слово")]
        public string FullWord { get; set; }

        [Display(Name = "Подсказка")]
        public string Hint { get; set; }

        [Display(Name = "Порядок")]
        public int OrderIndex { get; set; }

        public int TestId { get; set; }
        public virtual SpellingTest Test { get; set; }
    }
}
