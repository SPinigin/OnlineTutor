using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models
{
    public class MaterialTopic
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Тема материала")]
        public string Name { get; set; }

        // Навигационное свойство: список материалов в этой теме
        public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
    }
}