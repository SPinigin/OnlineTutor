using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models
{
    public class Material
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        [Display(Name = "Заголовок")]
        public string Title { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Тип материала")]
        public MaterialType Type { get; set; }

        [Required]
        [Display(Name = "Путь к файлу")]
        public string FileUrl { get; set; }

        [Required]
        [Display(Name = "Тема")]
        public int TopicId { get; set; }
        public virtual MaterialTopic Topic { get; set; }

        // К каким классам или ученикам привязано (необязательно)
        [Display(Name = "Класс")]
        public int? ClassId { get; set; }
        public virtual Class Class { get; set; }

        [Display(Name = "Ученик")]
        public int? StudentId { get; set; }
        public virtual User Student { get; set; }

        // Какому учителю принадлежит материал
        [Required]
        public int TeacherId { get; set; }
        public virtual User Teacher { get; set; }

        [Display(Name = "Дата загрузки")]
        public DateTime UploadedAt { get; set; } = DateTime.Now;
    }

    //public enum MaterialType
    //{
    //    [Display(Name = "Документ")]
    //    Document,
    //    [Display(Name = "Видео")]
    //    Video,
    //    [Display(Name = "Ссылка")]
    //    Link,
    //    [Display(Name = "Изображение")]
    //    Image
    //}
}