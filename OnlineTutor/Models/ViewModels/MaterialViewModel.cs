using Microsoft.AspNetCore.Http;
using OnlineTutor.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models.ViewModels
{
    public class MaterialViewModel
    {
        [Required]
        [Display(Name = "Заголовок")]
        public string Title { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Тема")]
        public int TopicId { get; set; }
        public List<MaterialTopic> Topics { get; set; }

        [Display(Name = "Тип материала")]
        public MaterialType Type { get; set; }

        [Display(Name = "Файл")]
        public IFormFile File { get; set; }

        [Display(Name = "Или URL (для ссылок)")]
        public string FileUrl { get; set; }

        [Display(Name = "Класс")]
        public int? ClassId { get; set; }
        public List<Class> Classes { get; set; }

        [Display(Name = "Ученик")]
        public int? StudentId { get; set; }
        public List<User> Students { get; set; }
    }
}
