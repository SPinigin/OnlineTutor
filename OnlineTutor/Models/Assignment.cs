using OnlineTutor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models
{
    public class Assignment
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public AssignmentType Type { get; set; }

        public int ClassId { get; set; }
        public virtual Class Class { get; set; }

        public int TeacherId { get; set; }  // Изменено с string на int
        public virtual User Teacher { get; set; }

        public virtual ICollection<StudentAssignment> StudentAssignments { get; set; } = new List<StudentAssignment>();
        public virtual ICollection<AssignmentMaterial> Materials { get; set; } = new List<AssignmentMaterial>();

        public enum AssignmentType
        {
            Test,
            Essay,
            ReadingMaterial,
            CreativeTask
        }
    }
}
