using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models.ViewModels
{
    public class SpellingWordViewModel
    {
        public int Id { get; set; }
        public int TestId { get; set; }

        [Required(ErrorMessage = "Слово с пропуском обязательно")]
        [Display(Name = "Слово с пропуском")]
        public string WordWithGap { get; set; }

        [Required(ErrorMessage = "Правильная буква обязательна")]
        [Display(Name = "Правильная буква")]
        public string CorrectLetter { get; set; }

        [Required(ErrorMessage = "Полное слово обязательно")]
        [Display(Name = "Полное слово")]
        public string FullWord { get; set; }

        [Display(Name = "Подсказка")]
        public string Hint { get; set; }

        [Display(Name = "Порядок")]
        public int OrderIndex { get; set; }
    }
}
