using OnlineTutor.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models
{
    public class Class
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Название класса")]
        public string Name { get; set; }
        
        [Required]
        [Display(Name = "Учебный год")]
        public int Year { get; set; }

        [Required]
        public int TeacherId { get; set; }
        
        public virtual User Teacher { get; set; }

        public virtual ICollection<StudentProfile> Students { get; set; } = new List<StudentProfile>();
        public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}
