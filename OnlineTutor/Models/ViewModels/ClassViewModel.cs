using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models.ViewModels
{
    public class ClassViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название класса обязательно")]
        [Display(Name = "Название класса")]
        [StringLength(50, ErrorMessage = "Название класса не может быть длиннее 50 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Учебный год обязателен")]
        [Display(Name = "Учебный год")]
        [Range(2000, 2100, ErrorMessage = "Учебный год должен быть между 2000 и 2100")]
        public int Year { get; set; }
    }
}
