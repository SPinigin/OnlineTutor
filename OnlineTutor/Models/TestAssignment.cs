using System.ComponentModel.DataAnnotations;

namespace OnlineTutor.Models
{
    public class TestAssignment
    {
        public int Id { get; set; }

        [Required]
        public DateTime AssignedDate { get; set; }

        public DateTime? DueDate { get; set; }

        [Required]
        public int TestId { get; set; }

        public virtual Test Test { get; set; }

        public int? ClassId { get; set; }

        public virtual Class Class { get; set; }

        public int? StudentId { get; set; }  // Изменено с string на int

        public virtual User Student { get; set; }

        public virtual ICollection<TestAttempt> TestAttempts { get; set; } = new List<TestAttempt>();
    }
}
