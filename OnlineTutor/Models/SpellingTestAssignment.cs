using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models
{
    public class SpellingTestAssignment
    {
        public int Id { get; set; }

        [Required]
        public DateTime AssignedDate { get; set; }

        public DateTime? DueDate { get; set; }

        [Required]
        public int TestId { get; set; }
        public virtual SpellingTest Test { get; set; }

        public int? ClassId { get; set; }
        public virtual Class Class { get; set; }

        public int? StudentId { get; set; }
        public virtual User Student { get; set; }

        public virtual ICollection<SpellingTestAttempt> Attempts { get; set; } = new List<SpellingTestAttempt>();
    }
}
